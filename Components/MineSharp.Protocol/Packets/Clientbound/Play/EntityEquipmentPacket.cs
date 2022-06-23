using MineSharp.Core.Types;
namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class EntityEquipmentPacket : Packet {

        public int EntityID { get; private set; }
        public Slot? Equipment { get; private set; }
        public Slot? Item { get; private set; }

        public EntityEquipmentPacket() { }

        public EntityEquipmentPacket(int entityid, Slot equipment, Slot item) {
            this.EntityID = entityid;
            this.Equipment = equipment;
            this.Item = item;
        }

        public override void Read(PacketBuffer buffer) {
            this.EntityID = buffer.ReadVarInt();
            this.Equipment = buffer.ReadSlot();
            this.Item = buffer.ReadSlot();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.EntityID);
            buffer.WriteSlot(this.Equipment);
            buffer.WriteSlot(this.Item);
        }
    }
}