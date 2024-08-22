using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.NetworkTypes;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Change difficulty packet
/// </summary>
/// <param name="NewDifficulty">The new difficulty level</param>
public sealed partial record ChangeDifficultyPacket(DifficultyLevel NewDifficulty) : IPacketStatic<ChangeDifficultyPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_SetDifficulty;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteByte((byte)NewDifficulty);
    }

    /// <inheritdoc />
    public static ChangeDifficultyPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var newDifficulty = (DifficultyLevel)buffer.ReadByte();

        return new(newDifficulty);
    }
}
