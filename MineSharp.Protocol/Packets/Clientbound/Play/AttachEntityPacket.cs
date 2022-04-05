namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class AttachEntityPacket : Packet {

        public int AttachedEntityID { get; private set; }
        public int HoldingEntityID { get; private set; }

        public AttachEntityPacket() { }

        public AttachEntityPacket(int attachedentityid, int holdingentityid) {
            this.AttachedEntityID = attachedentityid;
            this.HoldingEntityID = holdingentityid;
        }

        public override void Read(PacketBuffer buffer) {
            this.AttachedEntityID = buffer.ReadInt();
            this.HoldingEntityID = buffer.ReadInt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteInt(this.AttachedEntityID);
            buffer.WriteInt(this.HoldingEntityID);
        }
    }
}