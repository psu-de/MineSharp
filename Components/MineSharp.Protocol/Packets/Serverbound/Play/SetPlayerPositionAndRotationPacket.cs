using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public class SetPlayerPositionAndRotationPacket : IPacket
{
    public SetPlayerPositionAndRotationPacket(double x, double y, double z, float yaw, float pitch, bool isOnGround)
    {
        X = x;
        Y = y;
        Z = z;
        Yaw = yaw;
        Pitch = pitch;
        IsOnGround = isOnGround;
    }

    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public float Yaw { get; set; }
    public float Pitch { get; set; }
    public bool IsOnGround { get; set; }
    public PacketType Type => PacketType.SB_Play_PositionLook;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteDouble(X);
        buffer.WriteDouble(Y);
        buffer.WriteDouble(Z);
        buffer.WriteFloat(Yaw);
        buffer.WriteFloat(Pitch);
        buffer.WriteBool(IsOnGround);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var x = buffer.ReadDouble();
        var y = buffer.ReadDouble();
        var z = buffer.ReadDouble();
        var yaw = buffer.ReadFloat();
        var pitch = buffer.ReadFloat();
        var isOnGround = buffer.ReadBool();
        return new SetPlayerPositionAndRotationPacket(x, y, z, yaw, pitch, isOnGround);
    }
}
#pragma warning restore CS1591
