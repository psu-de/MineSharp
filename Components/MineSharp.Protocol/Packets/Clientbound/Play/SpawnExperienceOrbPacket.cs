namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class SpawnExperienceOrbPacket : Packet {

        public int EntityId { get; private set; }
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }
        public short Count { get; private set; }

        public SpawnExperienceOrbPacket() { }

        public SpawnExperienceOrbPacket(int entityId, double x, double y, double z, short count) {
            EntityId = entityId;
            X = x;
            Y = y;
            Z = z;
            Count = count;
        }

        public override void Read(PacketBuffer buffer) {
            EntityId = buffer.ReadVarInt();
            X = buffer.ReadDouble();
            Y = buffer.ReadDouble();
            Z = buffer.ReadDouble();
            Count = buffer.ReadShort();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(EntityId);
            buffer.WriteDouble(X);
            buffer.WriteDouble(Y);
            buffer.WriteDouble(Z);
            buffer.WriteShort(Count);
        }
    }
}
