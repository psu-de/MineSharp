using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Configuration;
#pragma warning disable CS1591
public class ClientInformationPacket : IPacket
{
    public ClientInformationPacket(string locale, byte viewDistance, int chatMode, bool chatColors,
                                   byte displayedSkinParts, int mainHand,
                                   bool enableTextFiltering, bool allowServerListings)
    {
        Locale = locale;
        ViewDistance = viewDistance;
        ChatMode = chatMode;
        ChatColors = chatColors;
        DisplayedSkinParts = displayedSkinParts;
        MainHand = mainHand;
        EnableTextFiltering = enableTextFiltering;
        AllowServerListings = allowServerListings;
    }

    public string Locale { get; set; }
    public byte ViewDistance { get; set; }
    public int ChatMode { get; set; }
    public bool ChatColors { get; set; }
    public byte DisplayedSkinParts { get; set; }
    public int MainHand { get; set; }
    public bool EnableTextFiltering { get; set; }
    public bool AllowServerListings { get; set; }
    public PacketType Type => PacketType.SB_Configuration_Settings;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteString(Locale);
        buffer.WriteByte(ViewDistance);
        buffer.WriteVarInt(ChatMode);
        buffer.WriteBool(ChatColors);
        buffer.WriteByte(DisplayedSkinParts);
        buffer.WriteVarInt(MainHand);
        buffer.WriteBool(EnableTextFiltering);
        buffer.WriteBool(AllowServerListings);
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
