using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Updates the direction the player is looking in.
/// </summary>
/// <param name="Yaw">Absolute rotation on the X Axis, in degrees.</param>
/// <param name="Pitch">Absolute rotation on the Y Axis, in degrees.</param>
/// <param name="OnGround">True if the client is on the ground, false otherwise.</param>
public sealed record SetPlayerRotationPacket(float Yaw, float Pitch, bool OnGround) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_Look;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteFloat(Yaw);
        buffer.WriteFloat(Pitch);
        buffer.WriteBool(OnGround);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var yaw = buffer.ReadFloat();
        var pitch = buffer.ReadFloat();
        var onGround = buffer.ReadBool();

        return new SetPlayerRotationPacket(yaw, pitch, onGround);
    }
}
