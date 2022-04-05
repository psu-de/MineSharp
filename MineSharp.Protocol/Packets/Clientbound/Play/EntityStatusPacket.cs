namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class EntityStatusPacket : Packet {

        public int EntityID { get; private set; }
public byte EntityStatus { get; private set; }

        public EntityStatusPacket() { }

        public EntityStatusPacket(int entityid, byte entitystatus) {
            this.EntityID = entityid;
this.EntityStatus = entitystatus;
        }

        public override void Read(PacketBuffer buffer) {
            this.EntityID = buffer.ReadInt();
this.EntityStatus = buffer.ReadByte();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteInt(this.EntityID);
buffer.WriteByte(this.EntityStatus);
        }
    }
}