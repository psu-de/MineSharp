using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     End Combat packet
/// </summary>
/// <param name="Duration">Length of the combat in ticks</param>
public sealed partial record EndCombatPacket(int Duration) : IPacketStatic<EndCombatPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_EndCombatEvent;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(Duration);
    }

    /// <inheritdoc />
    public static EndCombatPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var duration = buffer.ReadVarInt();
        return new EndCombatPacket(duration);
    }
}
