using fNbt;
namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class NBTQueryResponsePacket : Packet {

        public int TransactionID { get; private set; }
        public NbtCompound? NBT { get; private set; }

        public NBTQueryResponsePacket() { }

        public NBTQueryResponsePacket(int transactionid, NbtCompound? nbt) {
            this.TransactionID = transactionid;
            this.NBT = nbt;
        }

        public override void Read(PacketBuffer buffer) {
            this.TransactionID = buffer.ReadVarInt();
            this.NBT = buffer.ReadNBTCompound();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.TransactionID);
            buffer.WriteNBTCompound(this.NBT);
        }
    }
}