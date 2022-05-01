using MineSharp.Core.Types;

namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class EntityPropertiesPacket : Packet {

        public int EntityID { get; private set; }
        public int Count { get; private set; }

        public List<(Identifier, double, List<(UUID, double, byte)>)> Data = new List<(Identifier, double, List<(UUID, double, byte)>)>();

        public EntityPropertiesPacket() { }

        public EntityPropertiesPacket(int entityid, int count, List<(Identifier, double, List<(UUID, double, byte)>)> data) {
            this.EntityID = entityid;
            this.Count = count;
            this.Data = data;
        }

        public override void Read(PacketBuffer buffer) {
            this.Data = new List<(Identifier, double, List<(UUID, double, byte)>)>();

            this.EntityID = buffer.ReadVarInt();
            this.Count = buffer.ReadVarInt();

            for (int i = 0; i < this.Count; i++) {
                Identifier key = buffer.ReadIdentifier();
                double value = buffer.ReadDouble();
                int nModifiers = buffer.ReadVarInt();
                List<(UUID, double, byte)> modifiers = new List<(UUID, double, byte)>();

                for (int j = 0; j < nModifiers; j++) {
                    UUID uuid  = buffer.ReadUUID();
                    double amount = buffer.ReadDouble();
                    byte operation = buffer.ReadByte();
                    modifiers.Add((uuid, amount, operation));
                }
                this.Data.Add((key, value, modifiers));
            }
        }

        public override void Write(PacketBuffer buffer) {
            throw new NotSupportedException();
        }
    }
}