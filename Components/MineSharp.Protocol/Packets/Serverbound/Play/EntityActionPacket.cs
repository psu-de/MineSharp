namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class EntityActionPacket : Packet {

        public int EntityID { get; private set; }
public int /* TODO: Enum! */ ActionID { get; private set; }
public int JumpBoost { get; private set; }

        public EntityActionPacket() { }

        public EntityActionPacket(int entityid, int /* TODO: Enum! */ actionid, int jumpboost) {
            this.EntityID = entityid;
this.ActionID = actionid;
this.JumpBoost = jumpboost;
        }

        public override void Read(PacketBuffer buffer) {
            this.EntityID = buffer.ReadVarInt();
this.ActionID = buffer.ReadVarInt();
this.JumpBoost = buffer.ReadVarInt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.EntityID);
buffer.WriteVarInt(this.ActionID);
buffer.WriteVarInt(this.JumpBoost);
        }
    }
}