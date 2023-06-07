using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

public class RemoveEntitiesPacket : IPacket
{
    public static int Id => 0x3E;
    
    public int[] EntityIds { get; set; }

    public RemoveEntitiesPacket(int[] entityIds)
    {
        this.EntityIds = entityIds;
    }

    public void Write(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        buffer.WriteVarIntArray(this.EntityIds, (buf, i) => buf.WriteVarInt(i));
    }
    public static IPacket Read(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        var entityIds = buffer.ReadVarIntArray(buf => buf.ReadVarInt());
        return new RemoveEntitiesPacket(entityIds);
    }
}
