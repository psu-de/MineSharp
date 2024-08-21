using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Select Trade packet sent by the client when a player selects a specific trade offered by a villager NPC.
/// </summary>
/// <param name="SelectedSlot">The selected slot in the player's current (trading) inventory.</param>
public sealed partial record SelectTradePacket(int SelectedSlot) : IPacketStatic<SelectTradePacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_SelectTrade;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(SelectedSlot);
    }

    /// <inheritdoc />
    public static SelectTradePacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var selectedSlot = buffer.ReadVarInt();

        return new(selectedSlot);
    }
}
