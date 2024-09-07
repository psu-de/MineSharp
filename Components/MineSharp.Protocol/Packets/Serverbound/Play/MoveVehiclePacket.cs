using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Sent when a player moves in a vehicle. Fields are the same as in Set Player Position and Rotation.
///     Note that all fields use absolute positioning and do not allow for relative positioning.
/// </summary>
/// <param name="X">Absolute position (X coordinate).</param>
/// <param name="Y">Absolute position (Y coordinate).</param>
/// <param name="Z">Absolute position (Z coordinate).</param>
/// <param name="Yaw">Absolute rotation on the vertical axis, in degrees.</param>
/// <param name="Pitch">Absolute rotation on the horizontal axis, in degrees.</param>
public sealed record MoveVehiclePacket(double X, double Y, double Z, float Yaw, float Pitch) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_VehicleMove;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteDouble(X);
        buffer.WriteDouble(Y);
        buffer.WriteDouble(Z);
        buffer.WriteFloat(Yaw);
        buffer.WriteFloat(Pitch);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var x = buffer.ReadDouble();
        var y = buffer.ReadDouble();
        var z = buffer.ReadDouble();
        var yaw = buffer.ReadFloat();
        var pitch = buffer.ReadFloat();

        return new MoveVehiclePacket(x, y, z, yaw, pitch);
    }
}
