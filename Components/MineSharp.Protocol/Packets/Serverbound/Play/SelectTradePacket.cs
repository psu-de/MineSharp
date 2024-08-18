using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Select Trade packet sent by the client when a player selects a specific trade offered by a villager NPC.
/// </summary>
/// <param name="SelectedSlot">The selected slot in the player's current (trading) inventory.</param>
public sealed record SelectTradePacket(int SelectedSlot) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_SelectTrade;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(SelectedSlot);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var selectedSlot = buffer.ReadVarInt();

        return new SelectTradePacket(selectedSlot);
    }
}
