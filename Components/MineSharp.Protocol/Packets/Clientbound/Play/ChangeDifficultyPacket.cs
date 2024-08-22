using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.NetworkTypes;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Change difficulty packet
/// </summary>
/// <param name="Difficulty">The difficulty setting</param>
/// <param name="DifficultyLocked">Whether the difficulty is locked</param>
public sealed record ChangeDifficultyPacket(DifficultyLevel Difficulty, bool DifficultyLocked) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_Difficulty;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.Write((byte)Difficulty);
        buffer.WriteBool(DifficultyLocked);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var difficulty = (DifficultyLevel)buffer.ReadByte();
        var difficultyLocked = buffer.ReadBool();

        return new ChangeDifficultyPacket(difficulty, difficultyLocked);
    }
}
