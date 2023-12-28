using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;
#pragma warning disable CS1591
public class SetPlayerPositionPacket : IPacket
{
    public PacketType Type => PacketType.SB_Play_Position;

    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public bool IsOnGround { get; set; }

    public SetPlayerPositionPacket(double x, double y, double z, bool isOnGround)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
        this.IsOnGround = isOnGround;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteDouble(this.X);
        buffer.WriteDouble(this.Y);
        buffer.WriteDouble(this.Z);
        buffer.WriteBool(this.IsOnGround);
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