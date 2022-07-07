using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Data.Protocol {

    public interface IPacketPayload {
        public void Read(PacketBuffer buffer);
        public void Write(PacketBuffer buffer);
    }

    public abstract partial class Packet : IPacketPayload {

        public int PacketId { get; private set; }

        public Packet(int packetId) {
            PacketId = packetId;
        }

        public abstract void Read(PacketBuffer buffer);
        public abstract void Write(PacketBuffer buffer);

    }

    public abstract partial class PacketBuffer {

    }
}
