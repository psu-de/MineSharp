using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Lock Difficulty packet
/// </summary>
/// <param name="Locked">Indicates if the difficulty is locked</param>
public sealed record LockDifficultyPacket(bool Locked) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_LockDifficulty;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteBool(Locked);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var locked = buffer.ReadBool();

        return new LockDifficultyPacket(locked);
    }
}
