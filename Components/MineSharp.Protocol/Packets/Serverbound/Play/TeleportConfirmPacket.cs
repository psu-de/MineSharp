namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class TeleportConfirmPacket : Packet {

        public int TeleportID { get; private set; }

        public TeleportConfirmPacket() { }

        public TeleportConfirmPacket(int teleportid) {
            this.TeleportID = teleportid;
        }

        public override void Read(PacketBuffer buffer) {
            this.TeleportID = buffer.ReadVarInt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.TeleportID);
        }
    }
}