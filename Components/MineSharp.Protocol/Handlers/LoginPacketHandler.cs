using MineSharp.Core.Logging;
using MineSharp.Core.Types.Enums;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Crypto;
using System.Numerics;
using System.Security.Cryptography;
using MineSharp.Data.Protocol.Login.Clientbound;
using System.Text;

namespace MineSharp.Protocol.Handlers {
    internal class LoginPacketHandler : IPacketHandler {

        private static Logger Logger = Logger.GetLogger();

        public Task HandleIncomming(IPacketPayload packet, MinecraftClient client) {
            return packet switch {
                PacketDisconnect disconnect => HandleDisconnectPacket(disconnect, client),
                Data.Protocol.Login.Clientbound.PacketEncryptionBegin encryptionBegin => HandleEncryptionBeginPacket(encryptionBegin, client),
                PacketCompress compress => HandleCompressPacket(compress, client),
                PacketLoginPluginRequest loginPluginRequest => HandleLoginPluginRequestPacket(loginPluginRequest, client),
                PacketSuccess success => HandleSuccessPacket(success, client),
                _ => Task.CompletedTask
            };
        }

        public Task HandleOutgoing(IPacketPayload packet, MinecraftClient client) {
            return packet switch {
                Data.Protocol.Login.Serverbound.PacketEncryptionBegin encryptionBegin => HandleOutgoingEncryptionBeginPacket(encryptionBegin, client),
                _ => Task.CompletedTask
            };
        }

        #region Handle Incomming

        private Task HandleSuccessPacket(PacketSuccess packet, MinecraftClient client) {
            client.SetGameState(GameState.PLAY);
            return Task.CompletedTask;
        }

        private Task HandleLoginPluginRequestPacket(PacketLoginPluginRequest packet, MinecraftClient client) {
            throw new NotImplementedException();
        }

        private Task HandleCompressPacket(PacketCompress packet, MinecraftClient client) {
            Logger.Debug("Enabling compression, threshold=" + packet.Threshold!);
            client.SetCompressionThreshold(packet.Threshold!);
            return Task.CompletedTask;
        }

        private async Task HandleEncryptionBeginPacket(Data.Protocol.Login.Clientbound.PacketEncryptionBegin packet, MinecraftClient client) {
            Aes aes = Aes.Create();
            aes.KeySize = 128;
            aes.GenerateKey();
            client.SetEncryptionKey(aes.Key);

            byte[] hash = SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(packet.ServerId).Concat(aes.Key).Concat(packet.PublicKey!).ToArray());
            Array.Reverse(hash);
            BigInteger b = new BigInteger(hash);
            // very annoyingly, BigInteger in C# tries to be smart and puts in
            // a leading 0 when formatting as a hex number to allow roundtripping 
            // of negative numbers, thus we have to trim it off.
            string hex = "";
            if (b < 0) {
                // toss in a negative sign if the interpreted number is negative
                hex = "-" + (-b).ToString("x").TrimStart('0');
            } else {
                hex = b.ToString("x").TrimStart('0');
            }


            // Authenticate
            if (client.Session.OnlineSession) {
                if (!await client.Session.JoinServer(hex)) {
                    throw new Exception("Error trying to authenticate with mojang");
                }
            }

            RSA? rsa = RSAHelper.DecodePublicKey(packet.PublicKey!);
            if (rsa == null) {
                throw new Exception("Could not decode public key");
            }
            byte[] encrypted = rsa.Encrypt(aes.Key, RSAEncryptionPadding.Pkcs1);
            byte[] encVerTok = rsa.Encrypt(packet.VerifyToken!, RSAEncryptionPadding.Pkcs1);

            Data.Protocol.Login.Serverbound.PacketEncryptionBegin response = new Data.Protocol.Login.Serverbound.PacketEncryptionBegin(encrypted, encVerTok);
            _ = client.SendPacket(response); // TODO: Should this be awaited?
        }

        private Task HandleDisconnectPacket(PacketDisconnect packet, MinecraftClient client) {
            client.ForceDisconnect(packet.Reason!);
            return Task.CompletedTask;
        }

        #endregion

        #region Handle Outgoing

        private Task HandleOutgoingEncryptionBeginPacket(Data.Protocol.Login.Serverbound.PacketEncryptionBegin packet, MinecraftClient client) {
            client.EnableEncryption();
            return Task.CompletedTask;
        }

        #endregion
    }
}
