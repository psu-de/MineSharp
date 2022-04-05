namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class SetCooldownPacket : Packet {

        public int ItemID { get; private set; }
public int CooldownTicks { get; private set; }

        public SetCooldownPacket() { }

        public SetCooldownPacket(int itemid, int cooldownticks) {
            this.ItemID = itemid;
this.CooldownTicks = cooldownticks;
        }

        public override void Read(PacketBuffer buffer) {
            this.ItemID = buffer.ReadVarInt();
this.CooldownTicks = buffer.ReadVarInt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.ItemID);
buffer.WriteVarInt(this.CooldownTicks);
        }
    }
}