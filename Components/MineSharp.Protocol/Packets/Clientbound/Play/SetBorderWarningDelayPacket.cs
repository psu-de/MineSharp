using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Packet sent by the server to set the border warning delay.
/// </summary>
/// <param name="WarningTime">The warning time in seconds as set by /worldborder warning time.</param>
public sealed record SetBorderWarningDelayPacket(int WarningTime) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_WorldBorderWarningDelay;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(WarningTime);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var warningTime = buffer.ReadVarInt();
        return new SetBorderWarningDelayPacket(warningTime);
    }
}
