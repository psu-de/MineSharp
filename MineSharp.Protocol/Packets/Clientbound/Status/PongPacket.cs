using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Protocol.Packets.Clientbound.Status {
    public class PongPacket : Packet {

        public long Payload { get; private set; }

        public PongPacket() { }

        public PongPacket(long payload) {
            Payload = payload;
        }

        public override void Read(PacketBuffer buffer) {
            this.Payload = buffer.ReadLong();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteLong(Payload);
        }
    }
}
