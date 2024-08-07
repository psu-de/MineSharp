using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Sent when the player changes the slot selection
/// </summary>
public class SetHeldItemPacket : IPacket
{
    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="slot"></param>
    public SetHeldItemPacket(short slot)
    {
        Slot = slot;
    }

    /// <summary>
    ///     Index of the new selected hotbar slot (0-8)
    /// </summary>
    public short Slot { get; set; }

    /// <inheritdoc />
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.SB_Play_HeldItemSlot;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteShort(Slot);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new SetHeldItemPacket(buffer.ReadShort());
    }
}
