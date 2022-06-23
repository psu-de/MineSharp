namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class ClientSettingsPacket : Packet {

        public string? Locale { get; private set; }
public byte ViewDistance { get; private set; }
public int /* TODO: Enum! */ ChatMode { get; private set; }
public bool ChatColors { get; private set; }
public byte DisplayedSkinParts { get; private set; }
public int /* TODO: Enum! */ MainHand { get; private set; }
public bool Enabletextfiltering { get; private set; }
public bool Allowserverlistings { get; private set; }

        public ClientSettingsPacket() { }

        public ClientSettingsPacket(string locale, byte viewdistance, int /* TODO: Enum! */ chatmode, bool chatcolors, byte displayedskinparts, int /* TODO: Enum! */ mainhand, bool enabletextfiltering, bool allowserverlistings) {
            this.Locale = locale;
this.ViewDistance = viewdistance;
this.ChatMode = chatmode;
this.ChatColors = chatcolors;
this.DisplayedSkinParts = displayedskinparts;
this.MainHand = mainhand;
this.Enabletextfiltering = enabletextfiltering;
this.Allowserverlistings = allowserverlistings;
        }

        public override void Read(PacketBuffer buffer) {
            this.Locale = buffer.ReadString();
this.ViewDistance = buffer.ReadByte();
this.ChatMode = buffer.ReadVarInt();
this.ChatColors = buffer.ReadBoolean();
this.DisplayedSkinParts = buffer.ReadByte();
this.MainHand = buffer.ReadVarInt();
this.Enabletextfiltering = buffer.ReadBoolean();
this.Allowserverlistings = buffer.ReadBoolean();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteString(this.Locale!);
buffer.WriteByte(this.ViewDistance);
buffer.WriteVarInt(this.ChatMode);
buffer.WriteBoolean(this.ChatColors);
buffer.WriteByte(this.DisplayedSkinParts);
buffer.WriteVarInt(this.MainHand);
buffer.WriteBoolean(this.Enabletextfiltering);
buffer.WriteBoolean(this.Allowserverlistings);
        }
    }
}