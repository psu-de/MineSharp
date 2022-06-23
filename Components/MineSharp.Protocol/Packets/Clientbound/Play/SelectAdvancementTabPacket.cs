namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class SelectAdvancementTabPacket : Packet {

        public bool Hasid { get; private set; }
public string? OptionalIdentifier { get; private set; }

        public SelectAdvancementTabPacket() { }

        public SelectAdvancementTabPacket(bool hasid, string optionalidentifier) {
            this.Hasid = hasid;
this.OptionalIdentifier = optionalidentifier;
        }

        public override void Read(PacketBuffer buffer) {
            this.Hasid = buffer.ReadBoolean();
this.OptionalIdentifier = buffer.ReadString();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteBoolean(this.Hasid);
buffer.WriteString(this.OptionalIdentifier!);
        }
    }
}