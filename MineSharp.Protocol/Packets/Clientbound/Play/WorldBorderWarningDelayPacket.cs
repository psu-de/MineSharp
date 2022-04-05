namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class WorldBorderWarningDelayPacket : Packet {

        public int WarningTime { get; private set; }

        public WorldBorderWarningDelayPacket() { }

        public WorldBorderWarningDelayPacket(int warningtime) {
            this.WarningTime = warningtime;
        }

        public override void Read(PacketBuffer buffer) {
            this.WarningTime = buffer.ReadVarInt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.WarningTime);
        }
    }
}