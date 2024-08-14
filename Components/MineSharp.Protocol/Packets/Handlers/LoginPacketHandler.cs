using System.Diagnostics;
using System.Security.Cryptography;
using MineSharp.Auth.Exceptions;
using MineSharp.Core;
using MineSharp.Core.Common.Protocol;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Cryptography;
using MineSharp.Protocol.Packets.Clientbound.Login;
using MineSharp.Protocol.Packets.Serverbound.Login;
using NLog;

namespace MineSharp.Protocol.Packets.Handlers;

internal sealed class LoginPacketHandler : GameStatePacketHandler
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private readonly MinecraftClient client;
    private readonly MinecraftData data;

    public LoginPacketHandler(MinecraftClient client, MinecraftData data)
        : base(GameState.Login)
    {
        this.client = client;
        this.data = data;
    }

    public override Task StateEntered()
    {
        var login = HandshakeProtocol.GetLoginPacket(data, client.Session);
        return client.SendPacket(login);
    }

    public override Task HandleIncoming(IPacket packet)
    {
        return packet switch
        {
            DisconnectPacket disconnect => HandleDisconnect(disconnect),
            EncryptionRequestPacket encryption => HandleEncryptionRequest(encryption),
            SetCompressionPacket compression => HandleSetCompression(compression),
            // TODO: handle LoginPluginRequestPacket
            LoginSuccessPacket success => HandleLoginSuccess(success),
            _ => throw new UnreachableException()
        };
    }

    public override bool HandlesIncoming(PacketType type)
    {
        return type is PacketType.CB_Login_Disconnect
            or PacketType.CB_Login_EncryptionBegin
            or PacketType.CB_Login_Compress
            or PacketType.CB_Login_LoginPluginRequest
            or PacketType.CB_Login_Success;
    }

    private Task HandleDisconnect(DisconnectPacket packet)
    {
        _ = Task.Run(() => client.Disconnect(packet.Reason));
        return Task.CompletedTask;
    }

    private async Task HandleEncryptionRequest(EncryptionRequestPacket packet)
    {
        var aes = Aes.Create();
        aes.KeySize = 128;
        aes.GenerateKey();

        var hex = EncryptionHelper.ComputeHash(packet.ServerId, aes.Key, packet.PublicKey);

        // Authenticate
        if (client.Session.OnlineSession)
        {
            if (!await client.Api!.JoinServer(hex, client.Session.SessionToken, client.Session.Uuid))
            {
                throw new MineSharpAuthException("failed to authenticate with mojang");
            }
        }

        var rsa = EncryptionHelper.DecodePublicKey(packet.PublicKey!);
        if (rsa == null)
        {
            throw new NullReferenceException("failed to decode public key");
        }

        var sharedSecret = rsa.Encrypt(aes.Key, RSAEncryptionPadding.Pkcs1);
        var encVerToken = rsa.Encrypt(packet.VerifyToken!, RSAEncryptionPadding.Pkcs1);

        EncryptionResponsePacket response;
        if (ProtocolVersion.IsBetween(data.Version.Protocol, ProtocolVersion.V_1_19, ProtocolVersion.V_1_19_2)
            && client.Session.OnlineSession
            && client.Session.Certificate is not null)
        {
            var salt = ((long)RandomNumberGenerator.GetInt32(int.MaxValue) << 32) |
                (uint)RandomNumberGenerator.GetInt32(int.MaxValue);

            var signData = new PacketBuffer(data.Version.Protocol);
            signData.WriteBytes(packet.VerifyToken);
            signData.WriteLong(salt);

            var signed = client.Session.Certificate.RsaPrivate.SignData(signData.GetBuffer(), HashAlgorithmName.SHA256,
                                                                        RSASignaturePadding.Pkcs1);
            var crypto = new EncryptionResponsePacket.CryptoContainer(salt, signed);

            response = new(sharedSecret, null, crypto);
        }
        else
        {
            response = new(sharedSecret, encVerToken, null);
        }

        _ = client.SendPacket(response)
                  .ContinueWith(_ => client.EnableEncryption(aes.Key));
    }

    private Task HandleSetCompression(SetCompressionPacket packet)
    {
        Logger.Debug($"Enabling compression, threshold = {packet.Threshold}.");
        client.SetCompression(packet.Threshold);
        return Task.CompletedTask;
    }

    private async Task HandleLoginSuccess(LoginSuccessPacket packet)
    {
        if (data.Version.Protocol < ProtocolVersion.V_1_20_2)
        {
            await client.ChangeGameState(GameState.Play);
        }
        else
        {
            await client.SendPacket(new LoginAcknowledgedPacket());
            await client.ChangeGameState(GameState.Configuration);
        }
    }
}
