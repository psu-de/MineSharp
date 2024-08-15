using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public sealed record ClientInformationPacket(
    string Locale,
    byte ViewDistance,
    ChatMode ChatMode,
    bool ChatColors,
    SkinPart DisplayedSkinParts,
    PlayerHand MainHand,
    bool EnableTextFiltering,
    bool AllowServerListings
) : IPacketStatic<ClientInformationPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_Settings;

    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteString(Locale);
        buffer.WriteByte(ViewDistance);
        buffer.WriteVarInt((int)ChatMode);
        buffer.WriteBool(ChatColors);
        buffer.WriteByte((byte)DisplayedSkinParts);
        buffer.WriteVarInt((int)MainHand);
        buffer.WriteBool(EnableTextFiltering);
        buffer.WriteBool(AllowServerListings);
    }

    public static ClientInformationPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        return new ClientInformationPacket(
            buffer.ReadString(),
            buffer.ReadByte(),
            (ChatMode)buffer.ReadVarInt(),
            buffer.ReadBool(),
            (SkinPart)buffer.ReadByte(),
            (PlayerHand)buffer.ReadVarInt(),
            buffer.ReadBool(),
            buffer.ReadBool()
        );
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
#pragma warning restore CS1591
