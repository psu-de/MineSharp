using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public sealed record RemoveEntitiesPacket(int[] EntityIds) : IPacketStatic<RemoveEntitiesPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_EntityDestroy;

    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarIntArray(EntityIds, (buf, i) => buf.WriteVarInt(i));
    }

    public static RemoveEntitiesPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var entityIds = buffer.ReadVarIntArray(buf => buf.ReadVarInt());
        return new RemoveEntitiesPacket(entityIds);
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
#pragma warning restore CS1591
