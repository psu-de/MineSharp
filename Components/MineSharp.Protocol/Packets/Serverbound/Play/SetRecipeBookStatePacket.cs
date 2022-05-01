namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class SetRecipeBookStatePacket : Packet {

        public int /* TODO: Enum! */ BookID { get; private set; }
public bool BookOpen { get; private set; }
public bool FilterActive { get; private set; }

        public SetRecipeBookStatePacket() { }

        public SetRecipeBookStatePacket(int /* TODO: Enum! */ bookid, bool bookopen, bool filteractive) {
            this.BookID = bookid;
this.BookOpen = bookopen;
this.FilterActive = filteractive;
        }

        public override void Read(PacketBuffer buffer) {
            this.BookID = buffer.ReadVarInt();
this.BookOpen = buffer.ReadBoolean();
this.FilterActive = buffer.ReadBoolean();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.BookID);
buffer.WriteBoolean(this.BookOpen);
buffer.WriteBoolean(this.FilterActive);
        }
    }
}