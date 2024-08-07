using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class TeleportEntityPacket : IPacket
{
    public TeleportEntityPacket(int entityId, double x, double y, double z, sbyte yaw, sbyte pitch, bool onGround)
    {
        EntityId = entityId;
        X = x;
        Y = y;
        Z = z;
        Yaw = yaw;
        Pitch = pitch;
        OnGround = onGround;
    }

    public int EntityId { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public sbyte Yaw { get; set; }
    public sbyte Pitch { get; set; }
    public bool OnGround { get; set; }
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.CB_Play_EntityTeleport;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(EntityId);
        buffer.WriteDouble(X);
        buffer.WriteDouble(Y);
        buffer.WriteDouble(Z);
        buffer.WriteSByte(Yaw);
        buffer.WriteSByte(Pitch);
        buffer.WriteBool(OnGround);
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
