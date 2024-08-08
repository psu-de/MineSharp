using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public sealed record SetHeldItemPacket(sbyte Slot) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_HeldItemSlot;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteSByte(Slot);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new SetHeldItemPacket(
            buffer.ReadSByte());
    }
}
#pragma warning restore CS1591
