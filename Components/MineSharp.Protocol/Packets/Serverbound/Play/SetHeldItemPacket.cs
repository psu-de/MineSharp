using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Sent when the player changes the slot selection
/// </summary>
public sealed record SetHeldItemPacket(short Slot) : IPacketStatic<SetHeldItemPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_HeldItemSlot;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteShort(Slot);
    }

    /// <inheritdoc />
    public static SetHeldItemPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        return new SetHeldItemPacket(buffer.ReadShort());
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
