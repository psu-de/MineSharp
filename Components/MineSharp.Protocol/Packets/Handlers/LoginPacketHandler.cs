using MineSharp.Auth.Exceptions;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Protocol;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Cryptography;
using MineSharp.Protocol.Packets.Clientbound.Login;
using MineSharp.Protocol.Packets.Serverbound.Login;
using NLog;
using System.Diagnostics;
using System.Security.Cryptography;
using MineSharp.Core;

namespace MineSharp.Protocol.Packets.Handlers;

internal class LoginPacketHandler : IPacketHandler
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private readonly MinecraftClient _client;
    private readonly MinecraftData   _data;

    public LoginPacketHandler(MinecraftClient client, MinecraftData data)
    {
        this._client = client;
        this._data   = data;
    }

    public Task HandleIncoming(IPacket packet)
    {
        return packet switch
        {
            DisconnectPacket disconnect        => HandleDisconnect(disconnect),
            EncryptionRequestPacket encryption => HandleEncryptionRequest(encryption),
            SetCompressionPacket compression   => HandleSetCompression(compression),
            LoginSuccessPacket success         => HandleLoginSuccess(success),
            _                                  => throw new UnreachableException()
        };
    }

    public Task HandleOutgoing(IPacket packet)
        => Task.CompletedTask;

    public bool HandlesIncoming(PacketType type)
        => type is PacketType.CB_Login_Disconnect
            or PacketType.CB_Login_EncryptionBegin
            or PacketType.CB_Login_Compress
            or PacketType.CB_Login_LoginPluginRequest
            or PacketType.CB_Login_Success;

    private Task HandleDisconnect(DisconnectPacket packet)
    {
        _ = Task.Run(() => this._client.Disconnect(packet.Reason.Json));
        return Task.CompletedTask;
    }

    private async Task HandleEncryptionRequest(EncryptionRequestPacket packet)
    {
        var aes = Aes.Create();
        aes.KeySize = 128;
        aes.GenerateKey();

        var hex = EncryptionHelper.ComputeHash(packet.ServerId, aes.Key, packet.PublicKey);

        // Authenticate
        if (this._client.Session.OnlineSession)
        {
            if (!await this._client.Api!.JoinServer(hex, this._client.Session.SessionToken, this._client.Session.UUID))
            {
                throw new MineSharpAuthException("Error trying to authenticate with Mojang");
            }
        }

        var rsa = EncryptionHelper.DecodePublicKey(packet.PublicKey!);
        if (rsa == null)
            throw new Exception("Could not decode public key");

        var sharedSecret = rsa.Encrypt(aes.Key, RSAEncryptionPadding.Pkcs1);
        var encVerToken  = rsa.Encrypt(packet.VerifyToken!, RSAEncryptionPadding.Pkcs1);

        EncryptionResponsePacket response;
        if (ProtocolVersion.IsBetween(this._data.Version.Protocol, ProtocolVersion.V_1_19, ProtocolVersion.V_1_19_2)
         && this._client.Session.OnlineSession
         && this._client.Session.Certificate is not null)
        {
            var salt = (long)RandomNumberGenerator.GetInt32(int.MaxValue) << 32 | (uint)RandomNumberGenerator.GetInt32(int.MaxValue);

            var signData = new PacketBuffer(this._data.Version.Protocol >= ProtocolVersion.V_1_20_2);
            signData.WriteBytes(packet.VerifyToken);
            signData.WriteLong(salt);

            var signed = this._client.Session.Certificate.RsaPrivate.SignData(signData.GetBuffer(), HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);
            var crypto = new EncryptionResponsePacket.CryptoContainer(salt, signed);

            response = new EncryptionResponsePacket(sharedSecret, null, crypto);
        }
        else
            response = new EncryptionResponsePacket(sharedSecret, encVerToken, null);

        _ = this._client.SendPacket(response)
                .ContinueWith(_ => this._client.EnableEncryption(aes.Key));
    }

    private Task HandleSetCompression(SetCompressionPacket packet)
    {
        Logger.Debug($"Enabling compression, threshold = {packet.Threshold}.");
        this._client.SetCompression(packet.Threshold);
        return Task.CompletedTask;
    }

    private Task HandleLoginSuccess(LoginSuccessPacket packet)
    {
        if (this._data.Version.Protocol < ProtocolVersion.V_1_20_2)
        {
            this._client.UpdateGameState(GameState.Play);
            return Task.CompletedTask;
        }

        _ = this._client.SendPacket(new AcknowledgeLoginPacket())
                .ContinueWith(_ => this._client.UpdateGameState(GameState.Configuration));
        return Task.CompletedTask;
    }
}
