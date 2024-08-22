using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public sealed partial record SetPlayerPositionPacket(double X, double Y, double Z, bool IsOnGround) : IPacketStatic<SetPlayerPositionPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_Position;

    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteDouble(X);
        buffer.WriteDouble(Y);
        buffer.WriteDouble(Z);
        buffer.WriteBool(IsOnGround);
    }

    public static SetPlayerPositionPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var x = buffer.ReadDouble();
        var y = buffer.ReadDouble();
        var z = buffer.ReadDouble();
        var isOnGround = buffer.ReadBool();
        return new SetPlayerPositionPacket(x, y, z, isOnGround);
    }
}
#pragma warning restore CS1591
