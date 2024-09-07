using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Initialize World Border packet
/// </summary>
/// <param name="X">The X coordinate of the world border center</param>
/// <param name="Z">The Z coordinate of the world border center</param>
/// <param name="OldDiameter">Current length of a single side of the world border, in meters</param>
/// <param name="NewDiameter">Target length of a single side of the world border, in meters</param>
/// <param name="Speed">Number of real-time milliseconds until New Diameter is reached</param>
/// <param name="PortalTeleportBoundary">Resulting coordinates from a portal teleport are limited to ±value</param>
/// <param name="WarningBlocks">Warning distance in meters</param>
/// <param name="WarningTime">Warning time in seconds</param>
public sealed record InitializeWorldBorderPacket(
    double X,
    double Z,
    double OldDiameter,
    double NewDiameter,
    long Speed,
    int PortalTeleportBoundary,
    int WarningBlocks,
    int WarningTime) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_InitializeWorldBorder;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteDouble(X);
        buffer.WriteDouble(Z);
        buffer.WriteDouble(OldDiameter);
        buffer.WriteDouble(NewDiameter);
        buffer.WriteVarLong(Speed);
        buffer.WriteVarInt(PortalTeleportBoundary);
        buffer.WriteVarInt(WarningBlocks);
        buffer.WriteVarInt(WarningTime);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var x = buffer.ReadDouble();
        var z = buffer.ReadDouble();
        var oldDiameter = buffer.ReadDouble();
        var newDiameter = buffer.ReadDouble();
        var speed = buffer.ReadVarLong();
        var portalTeleportBoundary = buffer.ReadVarInt();
        var warningBlocks = buffer.ReadVarInt();
        var warningTime = buffer.ReadVarInt();

        return new InitializeWorldBorderPacket(
            x,
            z,
            oldDiameter,
            newDiameter,
            speed,
            portalTeleportBoundary,
            warningBlocks,
            warningTime);
    }
}
