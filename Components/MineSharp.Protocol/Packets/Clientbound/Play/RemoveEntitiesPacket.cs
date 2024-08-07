using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class RemoveEntitiesPacket : IPacket
{
    public RemoveEntitiesPacket(int[] entityIds)
    {
        EntityIds = entityIds;
    }

    public int[] EntityIds { get; set; }
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.CB_Play_EntityDestroy;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarIntArray(EntityIds, (buf, i) => buf.WriteVarInt(i));
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var entityIds = buffer.ReadVarIntArray(buf => buf.ReadVarInt());
        return new RemoveEntitiesPacket(entityIds);
    }
}
#pragma warning restore CS1591
