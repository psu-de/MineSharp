using MineSharp.Core.Geometry;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     This packet is used for a number of actions and animations performed by blocks, usually non-persistent.
///     The client ignores the provided block type and instead uses the block state in their world.
/// </summary>
/// <param name="Location">Block coordinates.</param>
/// <param name="ActionId">Varies depending on block — see Block Actions.</param>
/// <param name="ActionParameter">Varies depending on block — see Block Actions.</param>
/// <param name="BlockType">The block type ID for the block. This value is unused by the Notchian client, as it will infer the type of block based on the given position.</param>
public sealed record BlockActionPacket(Position Location, byte ActionId, byte ActionParameter, int BlockType) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_BlockAction;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WritePosition(Location);
        buffer.WriteByte(ActionId);
        buffer.WriteByte(ActionParameter);
        buffer.WriteVarInt(BlockType);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var location = buffer.ReadPosition();
        var actionId = buffer.ReadByte();
        var actionParameter = buffer.ReadByte();
        var blockType = buffer.ReadVarInt();

        return new BlockActionPacket(location, actionId, actionParameter, blockType);
    }
}
