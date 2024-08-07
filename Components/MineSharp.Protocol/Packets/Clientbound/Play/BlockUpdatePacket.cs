using MineSharp.Core.Common;
using MineSharp.Core.Geometry;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
/// <summary>
///     Block update packet
/// </summary>
public class BlockUpdatePacket : IPacket
{
    /// <summary>
    ///     Create a new instance
    /// </summary>
    /// <param name="location"></param>
    /// <param name="stateId"></param>
    public BlockUpdatePacket(Position location, int stateId)
    {
        Location = location;
        StateId = stateId;
    }

    /// <summary>
    ///     The location of the block update
    /// </summary>
    public Position Location { get; set; }

    /// <summary>
    ///     The new state id
    /// </summary>
    public int StateId { get; set; }

    /// <inheritdoc />
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.CB_Play_BlockChange;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteULong(Location.ToULong());
        buffer.WriteVarInt(StateId);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var location = new Position(buffer.ReadULong());
        var stateId = buffer.ReadVarInt();
        return new BlockUpdatePacket(location, stateId);
    }
}
#pragma warning restore CS1591
