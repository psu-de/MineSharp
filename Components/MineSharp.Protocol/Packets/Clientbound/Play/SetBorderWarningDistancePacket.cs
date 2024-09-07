using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Packet sent by the server to set the border warning distance.
/// </summary>
/// <param name="WarningBlocks">The warning distance in meters.</param>
public sealed record SetBorderWarningDistancePacket(int WarningBlocks) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_WorldBorderWarningReach;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(WarningBlocks);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var warningBlocks = buffer.ReadVarInt();
        return new SetBorderWarningDistancePacket(warningBlocks);
    }
}
