using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public class SetPlayerPositionAndRotationPacket : IPacket
{
    public PacketType Type => PacketType.SB_Play_PositionLook;

    public double X          { get; set; }
    public double Y          { get; set; }
    public double Z          { get; set; }
    public float  Yaw        { get; set; }
    public float  Pitch      { get; set; }
    public bool   IsOnGround { get; set; }

    public SetPlayerPositionAndRotationPacket(double x, double y, double z, float yaw, float pitch, bool isOnGround)
    {
        this.X          = x;
        this.Y          = y;
        this.Z          = z;
        this.Yaw        = yaw;
        this.Pitch      = pitch;
        this.IsOnGround = isOnGround;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteDouble(this.X);
        buffer.WriteDouble(this.Y);
        buffer.WriteDouble(this.Z);
        buffer.WriteFloat(this.Yaw);
        buffer.WriteFloat(this.Pitch);
        buffer.WriteBool(this.IsOnGround);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var x          = buffer.ReadDouble();
        var y          = buffer.ReadDouble();
        var z          = buffer.ReadDouble();
        var yaw        = buffer.ReadFloat();
        var pitch      = buffer.ReadFloat();
        var isOnGround = buffer.ReadBool();
        return new SetPlayerPositionAndRotationPacket(x, y, z, yaw, pitch, isOnGround);
    }
}
#pragma warning restore CS1591
