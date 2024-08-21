using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

#pragma warning disable CS1591
public sealed partial record SetPassengersPacket(int EntityId, int[] PassengersId) : IPacketStatic<SetPassengersPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_SetPassengers;

    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(EntityId);
        buffer.WriteVarIntArray(PassengersId, (buf, elem) => buffer.WriteVarInt(elem));
    }

    public static SetPassengersPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        return new SetPassengersPacket(
            buffer.ReadVarInt(),
            buffer.ReadVarIntArray(buf => buf.ReadVarInt()));
    }
}
#pragma warning restore CS1591
