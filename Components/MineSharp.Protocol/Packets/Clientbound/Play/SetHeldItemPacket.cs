using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public sealed record SetHeldItemPacket(sbyte Slot) : IPacketStatic<SetHeldItemPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_HeldItemSlot;

    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteSByte(Slot);
    }

    public static SetHeldItemPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        return new SetHeldItemPacket(
            buffer.ReadSByte());
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
#pragma warning restore CS1591
