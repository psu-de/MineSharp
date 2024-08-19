using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Sent as a player is renaming an item in an anvil.
/// </summary>
/// <param name="ItemName">The new name of the item.</param>
public sealed record RenameItemPacket(string ItemName) : IPacketStatic<RenameItemPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_NameItem;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteString(ItemName);
    }

    /// <inheritdoc />
    public static RenameItemPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var itemName = buffer.ReadString();

        return new(itemName);
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
