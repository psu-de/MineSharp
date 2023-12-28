using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
/// <summary>
/// SpawnPlayerPacket used for versions &lt;= 1.20.1
/// Merged with SpawnEntityPacket in 1.20.2
/// </summary>
public class SpawnPlayerPacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_NamedEntitySpawn;
    
    public int EntityId { get; set; }
    public UUID PlayerUuid { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public byte Yaw { get; set; }
    public byte Pitch { get; set; }

    public SpawnPlayerPacket(int entityId, UUID playerUuid, double x, double y, double z, byte yaw, byte pitch)
    {
        this.EntityId = entityId;
        this.PlayerUuid = playerUuid;
        this.X = x;
        this.Y = y;
        this.Z = z;
        this.Yaw = yaw;
        this.Pitch = pitch;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(this.EntityId);
        buffer.WriteUuid(this.PlayerUuid);
        buffer.WriteDouble(this.X);
        buffer.WriteDouble(this.Y);
        buffer.WriteDouble(this.Z);
        buffer.WriteByte(this.Yaw);
        buffer.WriteByte(this.Pitch);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var entityId = buffer.ReadVarInt();
        var playerUuid = buffer.ReadUuid();
        var x = buffer.ReadDouble();
        var y = buffer.ReadDouble();
        var z = buffer.ReadDouble();
        var yaw = buffer.ReadByte();
        var pitch = buffer.ReadByte();
        return new SpawnPlayerPacket(entityId, playerUuid, x, y, z, yaw, pitch);
    }
}
#pragma warning restore CS1591