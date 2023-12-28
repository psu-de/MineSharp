using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class EntityStatusPacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_EntityStatus;

    public int EntityId { get; set; }
    public byte Status { get; set; }

    public EntityStatusPacket(int entityId, byte status)
    {
        EntityId = entityId;
        Status = status;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(this.EntityId);
        buffer.WriteByte(this.Status);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new EntityStatusPacket(
            buffer.ReadInt(),
            buffer.ReadByte());
    }
}
#pragma warning restore CS1591