using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Protocol.Packets.Serverbound.Login {
    public class EncryptionResponsePacket : Packet {

        public byte[] SharedSecret { get; private set; }
        public byte[] VerifyToken { get; private set; }

        public EncryptionResponsePacket() { }
        public EncryptionResponsePacket(byte[] sharedSecret, byte[] verifyToken) {
            SharedSecret = sharedSecret;
            VerifyToken = verifyToken;
        }
        public override void Read(PacketBuffer buffer) {
            this.SharedSecret = buffer.ReadByteArray();
            this.VerifyToken = buffer.ReadByteArray();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteByteArray(SharedSecret);
            buffer.WriteByteArray(VerifyToken);
        }

        public override async Task Sent(MinecraftClient client) {
            client.EnableEncryption();
            await base.Sent(client);
        }
    }
}
