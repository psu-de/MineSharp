using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Lock Difficulty packet
/// </summary>
/// <param name="Locked">Indicates if the difficulty is locked</param>
public sealed partial record LockDifficultyPacket(bool Locked) : IPacketStatic<LockDifficultyPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_LockDifficulty;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteBool(Locked);
    }

    /// <inheritdoc />
    public static LockDifficultyPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var locked = buffer.ReadBool();

        return new(locked);
    }
}
