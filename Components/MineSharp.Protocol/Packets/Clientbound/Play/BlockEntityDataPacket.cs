using fNbt;
using MineSharp.Core.Types;

namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class BlockEntityDataPacket : Packet {

        public Position? Location { get; private set; }
        public int Type { get; private set; }
        public NbtCompound? NBTData { get; private set; }

        public BlockEntityDataPacket() { }

        public BlockEntityDataPacket(Position location, int type, NbtCompound? nbtdata) {
            this.Location = location;
            this.Type = type;
            this.NBTData = nbtdata;
        }

        public override void Read(PacketBuffer buffer) {
            this.Location = buffer.ReadPosition();
            this.Type = buffer.ReadVarInt();
            this.NBTData = buffer.ReadNBTCompound();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WritePosition(this.Location!);
            buffer.WriteVarInt(this.Type);
            buffer.WriteNBTCompound(this.NBTData);
        }
    }
}