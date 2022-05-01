namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class WorldBorderWarningReachPacket : Packet {

        public int WarningBlocks { get; private set; }

        public WorldBorderWarningReachPacket() { }

        public WorldBorderWarningReachPacket(int warningblocks) {
            this.WarningBlocks = warningblocks;
        }

        public override void Read(PacketBuffer buffer) {
            this.WarningBlocks = buffer.ReadVarInt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.WarningBlocks);
        }
    }
}