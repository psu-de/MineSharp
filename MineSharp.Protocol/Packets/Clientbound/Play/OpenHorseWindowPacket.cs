namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class OpenHorseWindowPacket : Packet {

        public byte WindowID { get; private set; }
public int Slotcount { get; private set; }
public int EntityID { get; private set; }

        public OpenHorseWindowPacket() { }

        public OpenHorseWindowPacket(byte windowid, int slotcount, int entityid) {
            this.WindowID = windowid;
this.Slotcount = slotcount;
this.EntityID = entityid;
        }

        public override void Read(PacketBuffer buffer) {
            this.WindowID = buffer.ReadByte();
this.Slotcount = buffer.ReadVarInt();
this.EntityID = buffer.ReadInt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteByte(this.WindowID);
buffer.WriteVarInt(this.Slotcount);
buffer.WriteInt(this.EntityID);
        }
    }
}