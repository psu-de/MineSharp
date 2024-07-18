using MineSharp.Core.Common;
using MineSharp.Core.Geometry;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
/// <summary>
///     SpawnPaintingPacket used for versions &lt;= 1.18.2
/// </summary>
public class SpawnPaintingPacket : IPacket
{
    public SpawnPaintingPacket(int entityId, Uuid entityUuid, int title, Position location, sbyte direction)
    {
        EntityId = entityId;
        EntityUuid = entityUuid;
        Title = title;
        Location = location;
        Direction = direction;
    }


    public int EntityId { get; set; }
    public Uuid EntityUuid { get; set; }
    public int Title { get; set; }
    public Position Location { get; set; }
    public sbyte Direction { get; set; }
    public PacketType Type => PacketType.CB_Play_SpawnEntityPainting;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(EntityId);
        buffer.WriteUuid(EntityUuid);
        buffer.WriteVarInt(Title);
        buffer.WriteULong(Location.ToULong());
        buffer.WriteSByte(Direction);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var entityId = buffer.ReadVarInt();
        var entityUuid = buffer.ReadUuid();
        var title = buffer.ReadVarInt();
        var location = new Position(buffer.ReadULong());
        var direction = buffer.ReadSByte();
        return new SpawnPaintingPacket(entityId, entityUuid, title, location, direction);
    }
}
#pragma warning restore CS1591
