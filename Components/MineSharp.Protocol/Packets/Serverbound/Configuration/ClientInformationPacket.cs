using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Configuration;

/// <summary>
///     Client information packet
/// </summary>
/// <param name="Locale">The locale of the client</param>
/// <param name="ViewDistance">The view distance setting</param>
/// <param name="ChatMode">The chat mode setting</param>
/// <param name="ChatColors">Whether chat colors are enabled</param>
/// <param name="DisplayedSkinParts">The displayed skin parts</param>
/// <param name="MainHand">The main hand setting</param>
/// <param name="EnableTextFiltering">Whether text filtering is enabled</param>
/// <param name="AllowServerListings">Whether server listings are allowed</param>
public sealed record ClientInformationPacket(
    string Locale,
    byte ViewDistance,
    ChatMode ChatMode,
    bool ChatColors,
    SkinPart DisplayedSkinParts,
    PlayerHand MainHand,
    bool EnableTextFiltering,
    bool AllowServerListings
) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Configuration_Settings;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
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

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
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
}

