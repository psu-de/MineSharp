using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Enter Combat packet
/// </summary>
public sealed record EnterCombatPacket() : IPacketStatic<EnterCombatPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_EnterCombatEvent;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        // No fields to write
    }

    /// <inheritdoc />
    public static EnterCombatPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        // No fields to read
        return new EnterCombatPacket();
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
