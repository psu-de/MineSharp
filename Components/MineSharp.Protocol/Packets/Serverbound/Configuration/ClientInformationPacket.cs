using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Configuration;
#pragma warning disable CS1591
public class ClientInformationPacket : IPacket
{
    public PacketType Type => PacketType.SB_Configuration_Settings;

    public string Locale { get; set; }
    public byte ViewDistance { get; set; }
    public int ChatMode { get; set; }
    public bool ChatColors { get; set; }
    public byte DisplayedSkinParts { get; set; }
    public int MainHand { get; set; }
    public bool EnableTextFiltering { get; set; }
    public bool AllowServerListings { get; set; }

    public ClientInformationPacket(string locale, byte viewDistance, int chatMode, bool chatColors, byte displayedSkinParts, int mainHand, bool enableTextFiltering, bool allowServerListings)
    {
        this.Locale = locale;
        this.ViewDistance = viewDistance;
        this.ChatMode = chatMode;
        this.ChatColors = chatColors;
        this.DisplayedSkinParts = displayedSkinParts;
        this.MainHand = mainHand;
        this.EnableTextFiltering = enableTextFiltering;
        this.AllowServerListings = allowServerListings;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteString(this.Locale);
        buffer.WriteByte(this.ViewDistance);
        buffer.WriteVarInt(this.ChatMode);
        buffer.WriteBool(this.ChatColors);
        buffer.WriteByte(this.DisplayedSkinParts);
        buffer.WriteVarInt(this.MainHand);
        buffer.WriteBool(this.EnableTextFiltering);
        buffer.WriteBool(this.AllowServerListings);
    }
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new ClientInformationPacket(
            buffer.ReadString(),
            buffer.ReadByte(),
            buffer.ReadVarInt(),
            buffer.ReadBool(),
            buffer.ReadByte(),
            buffer.ReadVarInt(),
            buffer.ReadBool(),
            buffer.ReadBool());
    }
}
#pragma warning restore CS1591