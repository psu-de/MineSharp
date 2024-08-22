using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Change Container Slot State packet.
///     This packet is sent by the client when toggling the state of a Crafter.
/// </summary>
/// <param name="SlotId">The ID of the slot that was changed</param>
/// <param name="WindowId">The ID of the window that was changed</param>
/// <param name="State">The new state of the slot. True for enabled, false for disabled</param>
public sealed partial record ChangeContainerSlotStatePacket(int SlotId, int WindowId, bool State) : IPacketStatic<ChangeContainerSlotStatePacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_SetSlotState;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(SlotId);
        buffer.WriteVarInt(WindowId);
        buffer.WriteBool(State);
    }

    /// <inheritdoc />
    public static ChangeContainerSlotStatePacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var slotId = buffer.ReadVarInt();
        var windowId = buffer.ReadVarInt();
        var state = buffer.ReadBool();

        return new(slotId, windowId, state);
    }
}
