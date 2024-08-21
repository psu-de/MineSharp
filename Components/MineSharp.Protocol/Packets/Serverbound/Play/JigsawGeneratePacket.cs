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
public sealed partial record JigsawGeneratePacket(Position Location, int Levels, bool KeepJigsaws) : IPacketStatic<JigsawGeneratePacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_GenerateStructure;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WritePosition(Location);
        buffer.WriteVarInt(Levels);
        buffer.WriteBool(KeepJigsaws);
    }

    /// <inheritdoc />
    public static JigsawGeneratePacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var location = buffer.ReadPosition();
        var levels = buffer.ReadVarInt();
        var keepJigsaws = buffer.ReadBool();

        return new(location, levels, keepJigsaws);
    }
}
