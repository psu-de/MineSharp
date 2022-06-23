namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class ResourcePackSendPacket : Packet {

        public string? URL { get; private set; }
public string? Hash { get; private set; }
public bool Forced { get; private set; }
public bool HasPromptMessage { get; private set; }

        public ResourcePackSendPacket() { }

        public ResourcePackSendPacket(string url, string hash, bool forced, bool haspromptmessage) {
            this.URL = url;
this.Hash = hash;
this.Forced = forced;
this.HasPromptMessage = haspromptmessage;
        }

        public override void Read(PacketBuffer buffer) {
            this.URL = buffer.ReadString();
this.Hash = buffer.ReadString();
this.Forced = buffer.ReadBoolean();
this.HasPromptMessage = buffer.ReadBoolean();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteString(this.URL!);
buffer.WriteString(this.Hash!);
buffer.WriteBoolean(this.Forced);
buffer.WriteBoolean(this.HasPromptMessage);
        }
    }
}