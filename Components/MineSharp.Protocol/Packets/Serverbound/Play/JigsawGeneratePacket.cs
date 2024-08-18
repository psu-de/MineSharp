using MineSharp.Core.Geometry;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Sent when Generate is pressed on the Jigsaw Block interface.
/// </summary>
/// <param name="Location">Block entity location.</param>
/// <param name="Levels">Value of the levels slider/max depth to generate.</param>
/// <param name="KeepJigsaws">Whether to keep jigsaws.</param>
public sealed record JigsawGeneratePacket(Position Location, int Levels, bool KeepJigsaws) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_GenerateStructure;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WritePosition(Location);
        buffer.WriteVarInt(Levels);
        buffer.WriteBool(KeepJigsaws);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var location = buffer.ReadPosition();
        var levels = buffer.ReadVarInt();
        var keepJigsaws = buffer.ReadBool();

        return new JigsawGeneratePacket(location, levels, keepJigsaws);
    }
}
