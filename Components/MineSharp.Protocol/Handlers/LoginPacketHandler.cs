using MineSharp.Core.Logging;
using MineSharp.Core.Types.Enums;
using MineSharp.Data.Protocol;
using MineSharp.Data.Protocol.Login.Clientbound;
using MineSharp.Protocol.Crypto;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using PacketEncryptionBegin = MineSharp.Data.Protocol.Login.Serverbound.PacketEncryptionBegin;

namespace MineSharp.Protocol.Handlers
{
    internal class LoginPacketHandler : IPacketHandler
    {

        private static readonly Logger Logger = Logger.GetLogger();

        public Task HandleIncoming(IPacketPayload packet, MinecraftClient client)
        {
            return packet switch {
                PacketDisconnect disconnect => this.HandleDisconnectPacket(disconnect, client),
                Data.Protocol.Login.Clientbound.PacketEncryptionBegin encryptionBegin => this.HandleEncryptionBeginPacket(encryptionBegin, client),
                PacketCompress compress => this.HandleCompressPacket(compress, client),
                PacketLoginPluginRequest loginPluginRequest => this.HandleLoginPluginRequestPacket(loginPluginRequest, client),
                PacketSuccess success => this.HandleSuccessPacket(success, client),
                _ => Task.CompletedTask
            };
        }

        public Task HandleOutgoing(IPacketPayload packet, MinecraftClient client)
        {
            return packet switch {
                PacketEncryptionBegin encryptionBegin => this.HandleOutgoingEncryptionBeginPacket(encryptionBegin, client),
                _ => Task.CompletedTask
            };
        }

        #region Handle Outgoing


        private Task HandleOutgoingEncryptionBeginPacket(PacketEncryptionBegin packet, MinecraftClient client)
        {
            client.EnableEncryption();
            return Task.CompletedTask;
        }


        #endregion

        #region Handle Incomming


        private Task HandleSuccessPacket(PacketSuccess packet, MinecraftClient client)
        {
            client.SetGameState(GameState.PLAY);
            return Task.CompletedTask;
        }

        private Task HandleLoginPluginRequestPacket(PacketLoginPluginRequest packet, MinecraftClient client) => throw new NotImplementedException();

        private Task HandleCompressPacket(PacketCompress packet, MinecraftClient client)
        {
            Logger.Debug("Enabling compression, threshold=" + packet.Threshold!);
            client.SetCompressionThreshold(packet.Threshold!);
            return Task.CompletedTask;
        }

        private async Task HandleEncryptionBeginPacket(Data.Protocol.Login.Clientbound.PacketEncryptionBegin packet, MinecraftClient client)
        {
            var aes = Aes.Create();
            aes.KeySize = 128;
            aes.GenerateKey();
            client.SetEncryptionKey(aes.Key);

            var hash = SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(packet.ServerId).Concat(aes.Key).Concat(packet.PublicKey!).ToArray());
            Array.Reverse(hash);
            var b = new BigInteger(hash);
            // very annoyingly, BigInteger in C# tries to be smart and puts in
            // a leading 0 when formatting as a hex number to allow roundtripping 
            // of negative numbers, thus we have to trim it off.
            var hex = "";
            if (b < 0)
            {
                // toss in a negative sign if the interpreted number is negative
                hex = "-" + (-b).ToString("x").TrimStart('0');
            } else
            {
                hex = b.ToString("x").TrimStart('0');
            }


            // Authenticate
            if (client.Session.OnlineSession)
            {
                if (!await client.Session.JoinServer(hex))
                {
                    throw new Exception("Error trying to authenticate with mojang");
                }
            }

            var rsa = RSAHelper.DecodePublicKey(packet.PublicKey!);
            if (rsa == null)
            {
                throw new Exception("Could not decode public key");
            }
            var encrypted = rsa.Encrypt(aes.Key, RSAEncryptionPadding.Pkcs1);
            var encVerTok = rsa.Encrypt(packet.VerifyToken!, RSAEncryptionPadding.Pkcs1);

            var response = new PacketEncryptionBegin(encrypted, encVerTok);
            _ = client.SendPacket(response); // TODO: Should this be awaited?
        }

        private Task HandleDisconnectPacket(PacketDisconnect packet, MinecraftClient client)
        {
            client.ForceDisconnect(packet.Reason!);
            return Task.CompletedTask;
        }


        #endregion

    }
}
