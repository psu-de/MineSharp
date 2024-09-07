using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Display Objective packet sent to the client to display a scoreboard.
/// </summary>
/// <param name="Position">The position of the scoreboard.</param>
/// <param name="ScoreName">The unique name for the scoreboard to be displayed.</param>
public sealed record DisplayObjectivePacket(int Position, string ScoreName) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_ScoreboardDisplayObjective;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(Position);
        buffer.WriteString(ScoreName);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var position = buffer.ReadVarInt();
        var scoreName = buffer.ReadString();

        return new DisplayObjectivePacket(position, scoreName);
    }
}
