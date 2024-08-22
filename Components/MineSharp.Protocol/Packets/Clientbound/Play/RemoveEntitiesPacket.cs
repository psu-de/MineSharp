using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public sealed partial record RemoveEntitiesPacket(int[] EntityIds) : IPacketStatic<RemoveEntitiesPacket>
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
}
#pragma warning restore CS1591
