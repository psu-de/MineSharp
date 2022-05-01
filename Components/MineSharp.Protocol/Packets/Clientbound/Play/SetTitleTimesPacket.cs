namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class SetTitleTimesPacket : Packet {

        public int FadeIn { get; private set; }
public int Stay { get; private set; }
public int FadeOut { get; private set; }

        public SetTitleTimesPacket() { }

        public SetTitleTimesPacket(int fadein, int stay, int fadeout) {
            this.FadeIn = fadein;
this.Stay = stay;
this.FadeOut = fadeout;
        }

        public override void Read(PacketBuffer buffer) {
            this.FadeIn = buffer.ReadInt();
this.Stay = buffer.ReadInt();
this.FadeOut = buffer.ReadInt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteInt(this.FadeIn);
buffer.WriteInt(this.Stay);
buffer.WriteInt(this.FadeOut);
        }
    }
}