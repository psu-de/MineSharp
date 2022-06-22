using MineSharp.Protocol.Crypto;
using MineSharp.Protocol.Packets.Serverbound.Login;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace MineSharp.Protocol.Packets.Clientbound.Login {
    public class EncryptionRequestPacket : Packet {

        public string ServerID { get; private set; }
        public byte[] PublicKey { get; private set; }
        public byte[] VerifyToken { get; private set; }

        public EncryptionRequestPacket() { }
        public EncryptionRequestPacket(string serverId, byte[] publicKey, byte[] verifyToken) {
            ServerID = serverId;
            PublicKey = publicKey;
            VerifyToken = verifyToken; 
        }


        public override async Task Handle(MinecraftClient client) {
            Aes aes = Aes.Create();
            aes.KeySize = 128;
            aes.GenerateKey();
            client.SetEncryptionKey(aes.Key);

            byte[] hash = SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(ServerID).Concat(aes.Key).Concat(PublicKey).ToArray());
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

            RSA rsa = RSAHelper.DecodePublicKey(this.PublicKey);
            byte[] encrypted = rsa.Encrypt(aes.Key, RSAEncryptionPadding.Pkcs1);
            byte[] encVerTok = rsa.Encrypt(this.VerifyToken, RSAEncryptionPadding.Pkcs1);

            EncryptionResponsePacket response = new EncryptionResponsePacket(encrypted, encVerTok);
            client.SendPacket(response); // TODO: Should this be awaited?

            await base.Handle(client);
        }

        public override void Read(PacketBuffer buffer) {
            this.ServerID = buffer.ReadString();
            this.PublicKey = buffer.ReadByteArray();
            this.VerifyToken = buffer.ReadByteArray();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteString(ServerID);
            buffer.WriteByteArray(PublicKey);
            buffer.WriteByteArray(VerifyToken);
        }
    }
}
