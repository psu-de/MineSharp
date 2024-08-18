using MineSharp.Core.Common;
using MineSharp.Core.Geometry;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Sent when Done is pressed on the Jigsaw Block interface.
/// </summary>
/// <param name="Location">Block entity location</param>
/// <param name="Name">Name identifier</param>
/// <param name="Target">Target identifier</param>
/// <param name="Pool">Pool identifier</param>
/// <param name="FinalState">"Turns into" on the GUI, <c>final_state</c> in NBT</param>
/// <param name="JointType">Joint type, <c>rollable</c> if the attached piece can be rotated, else <c>aligned</c></param>
/// <param name="SelectionPriority">Selection priority</param>
/// <param name="PlacementPriority">Placement priority</param>
public sealed record ProgramJigsawBlockPacket(
    Position Location,
    Identifier Name,
    Identifier Target,
    Identifier Pool,
    string FinalState,
    string JointType,
    int SelectionPriority,
    int PlacementPriority) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_UpdateJigsawBlock;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WritePosition(Location);
        buffer.WriteIdentifier(Name);
        buffer.WriteIdentifier(Target);
        buffer.WriteIdentifier(Pool);
        buffer.WriteString(FinalState);
        buffer.WriteString(JointType);
        buffer.WriteVarInt(SelectionPriority);
        buffer.WriteVarInt(PlacementPriority);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var location = buffer.ReadPosition();
        var name = buffer.ReadIdentifier();
        var target = buffer.ReadIdentifier();
        var pool = buffer.ReadIdentifier();
        var finalState = buffer.ReadString();
        var jointType = buffer.ReadString();
        var selectionPriority = buffer.ReadVarInt();
        var placementPriority = buffer.ReadVarInt();

        return new ProgramJigsawBlockPacket(
            location,
            name,
            target,
            pool,
            finalState,
            jointType,
            selectionPriority,
            placementPriority);
    }
}
