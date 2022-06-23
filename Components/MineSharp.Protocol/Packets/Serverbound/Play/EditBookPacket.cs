namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class EditBookPacket : Packet {

        public int /* TODO: Enum! */ Hand { get; private set; }
public int Count { get; private set; }
public bool Hastitle { get; private set; }
public string? Title { get; private set; }

        public EditBookPacket() { }

        public EditBookPacket(int /* TODO: Enum! */ hand, int count, bool hastitle, string title) {
            this.Hand = hand;
this.Count = count;
this.Hastitle = hastitle;
this.Title = title;
        }

        public override void Read(PacketBuffer buffer) {
            this.Hand = buffer.ReadVarInt();
this.Count = buffer.ReadVarInt();
this.Hastitle = buffer.ReadBoolean();
this.Title = buffer.ReadString();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.Hand);
buffer.WriteVarInt(this.Count);
buffer.WriteBoolean(this.Hastitle);
buffer.WriteString(this.Title!);
        }
    }
}