using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class TeleportEntityPacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_EntityTeleport;
    
    public int EntityId { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public sbyte Yaw { get; set; }
    public sbyte Pitch { get; set; }
    public bool OnGround { get; set; }

    public TeleportEntityPacket(int entityId, double x, double y, double z, sbyte yaw, sbyte pitch, bool onGround)
    {
        this.EntityId = entityId;
        this.X = x;
        this.Y = y;
        this.Z = z;
        this.Yaw = yaw;
        this.Pitch = pitch;
        this.OnGround = onGround;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(this.EntityId);
        buffer.WriteDouble(this.X);
        buffer.WriteDouble(this.Y);
        buffer.WriteDouble(this.Z);
        buffer.WriteSByte(this.Yaw);
        buffer.WriteSByte(this.Pitch);
        buffer.WriteBool(this.OnGround);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var entityId = buffer.ReadVarInt();
        var x = buffer.ReadDouble();
        var y = buffer.ReadDouble();
        var z = buffer.ReadDouble();
        var yaw = buffer.ReadSByte();
        var pitch = buffer.ReadSByte();
        var onGround = buffer.ReadBool();

        return new TeleportEntityPacket(
            entityId, x, y, z, yaw, pitch, onGround);
    }
}
#pragma warning restore CS1591