using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Packet sent by the server to set the world border lerp size.
/// </summary>
/// <param name="OldDiameter">Current length of a single side of the world border, in meters.</param>
/// <param name="NewDiameter">Target length of a single side of the world border, in meters.</param>
/// <param name="Speed">Number of real-time milliseconds until New Diameter is reached.</param>
public sealed record SetBorderLerpSizePacket(double OldDiameter, double NewDiameter, long Speed) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_WorldBorderLerpSize;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteDouble(OldDiameter);
        buffer.WriteDouble(NewDiameter);
        buffer.WriteVarLong(Speed);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var oldDiameter = buffer.ReadDouble();
        var newDiameter = buffer.ReadDouble();
        var speed = buffer.ReadVarLong();

        return new SetBorderLerpSizePacket(oldDiameter, newDiameter, speed);
    }
}
