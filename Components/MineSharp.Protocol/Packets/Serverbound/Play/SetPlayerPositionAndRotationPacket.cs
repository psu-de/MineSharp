using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public sealed record SetPlayerPositionAndRotationPacket(double X, double Y, double Z, float Yaw, float Pitch, bool IsOnGround) : IPacketStatic<SetPlayerPositionAndRotationPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_PositionLook;

    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteDouble(X);
        buffer.WriteDouble(Y);
        buffer.WriteDouble(Z);
        buffer.WriteFloat(Yaw);
        buffer.WriteFloat(Pitch);
        buffer.WriteBool(IsOnGround);
    }

    public static SetPlayerPositionAndRotationPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var x = buffer.ReadDouble();
        var y = buffer.ReadDouble();
        var z = buffer.ReadDouble();
        var yaw = buffer.ReadFloat();
        var pitch = buffer.ReadFloat();
        var isOnGround = buffer.ReadBool();
        return new SetPlayerPositionAndRotationPacket(x, y, z, yaw, pitch, isOnGround);
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
#pragma warning restore CS1591
