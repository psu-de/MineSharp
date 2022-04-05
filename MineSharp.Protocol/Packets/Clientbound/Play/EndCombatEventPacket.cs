namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class EndCombatEventPacket : Packet {

        public int Duration { get; private set; }
public int EntityID { get; private set; }

        public EndCombatEventPacket() { }

        public EndCombatEventPacket(int duration, int entityid) {
            this.Duration = duration;
this.EntityID = entityid;
        }

        public override void Read(PacketBuffer buffer) {
            this.Duration = buffer.ReadVarInt();
this.EntityID = buffer.ReadInt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.Duration);
buffer.WriteInt(this.EntityID);
        }
    }
}