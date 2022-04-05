using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class KeepAlivePacket : Packet {

        public long KeepAliveID { get; private set; }

        public KeepAlivePacket() { }

        public KeepAlivePacket(long KeepAliveID) {
            this.KeepAliveID = KeepAliveID;
        }

        public override void Read(PacketBuffer buffer) {
            this.KeepAliveID = buffer.ReadLong();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteLong(KeepAliveID);
        }
    }
}
