using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
/// <summary>
/// SpawnPaintingPacket used for versions &lt;= 1.18.2
/// </summary>
public class SpawnPaintingPacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_SpawnEntityPainting;


    public int      EntityId   { get; set; }
    public UUID     EntityUuid { get; set; }
    public int      Title      { get; set; }
    public Position Location   { get; set; }
    public sbyte    Direction  { get; set; }


    public SpawnPaintingPacket(int entityId, UUID entityUuid, int title, Position location, sbyte direction)
    {
        this.EntityId   = entityId;
        this.EntityUuid = entityUuid;
        this.Title      = title;
        this.Location   = location;
        this.Direction  = direction;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(this.EntityId);
        buffer.WriteUuid(this.EntityUuid);
        buffer.WriteVarInt(this.Title);
        buffer.WriteULong(this.Location.ToULong());
        buffer.WriteSByte(this.Direction);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var entityId   = buffer.ReadVarInt();
        var entityUuid = buffer.ReadUuid();
        var title      = buffer.ReadVarInt();
        var location   = new Position(buffer.ReadULong());
        var direction  = buffer.ReadSByte();
        return new SpawnPaintingPacket(entityId, entityUuid, title, location, direction);
    }
}
#pragma warning restore CS1591
