using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.NetworkTypes;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Represents a packet that sets a slot in a window.
/// </summary>
/// <param name="WindowId">The ID of the window.</param>
/// <param name="StateId">The state ID of the window.</param>
/// <param name="Slot">The slot to be set.</param>
public sealed record WindowSetSlotPacket(sbyte WindowId, int StateId, Slot Slot) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_SetSlot;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteSByte(WindowId);
        buffer.WriteVarInt(StateId);
        buffer.WriteSlot(Slot);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new WindowSetSlotPacket(
            buffer.ReadSByte(),
            buffer.ReadVarInt(),
            buffer.ReadSlot(version));
    }
}

