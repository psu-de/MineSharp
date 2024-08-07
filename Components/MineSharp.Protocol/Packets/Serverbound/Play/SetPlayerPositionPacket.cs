using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public class SetPlayerPositionPacket : IPacket
{
    public SetPlayerPositionPacket(double x, double y, double z, bool isOnGround)
    {
        X = x;
        Y = y;
        Z = z;
        IsOnGround = isOnGround;
    }

    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public bool IsOnGround { get; set; }
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.SB_Play_Position;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteDouble(X);
        buffer.WriteDouble(Y);
        buffer.WriteDouble(Z);
        buffer.WriteBool(IsOnGround);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var x = buffer.ReadDouble();
        var y = buffer.ReadDouble();
        var z = buffer.ReadDouble();
        var isOnGround = buffer.ReadBool();
        return new SetPlayerPositionPacket(x, y, z, isOnGround);
    }
}
#pragma warning restore CS1591
