using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class RemoveEntitiesPacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_EntityDestroy;
    
    public int[] EntityIds { get; set; }

    public RemoveEntitiesPacket(int[] entityIds)
    {
        this.EntityIds = entityIds;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarIntArray(this.EntityIds, (buf, i) => buf.WriteVarInt(i));
    }
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var entityIds = buffer.ReadVarIntArray(buf => buf.ReadVarInt());
        return new RemoveEntitiesPacket(entityIds);
    }
}
#pragma warning restore CS1591