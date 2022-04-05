namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class EntityMetadataPacket : Packet {

        public int EntityID { get; private set; }
        public byte[] Data { get; private set; }


        public EntityMetadataPacket() { }

        public EntityMetadataPacket(int entityid, byte[] data) {
            this.EntityID = entityid;
            this.Data = data;
        }

        public override void Read(PacketBuffer buffer) {
            this.EntityID = buffer.ReadVarInt();
            this.Data = buffer.ReadRaw((int)buffer.ReadableBytes);
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.EntityID);
            buffer.WriteRaw(this.Data);
        }
    }
}