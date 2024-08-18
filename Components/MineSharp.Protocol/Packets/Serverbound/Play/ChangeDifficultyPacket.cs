using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.NetworkTypes;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Change difficulty packet
/// </summary>
/// <param name="NewDifficulty">The new difficulty level</param>
public sealed record ChangeDifficultyPacket(DifficultyLevel NewDifficulty) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_SetDifficulty;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteByte((byte)NewDifficulty);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var newDifficulty = (DifficultyLevel)buffer.ReadByte();

        return new ChangeDifficultyPacket(newDifficulty);
    }
}
