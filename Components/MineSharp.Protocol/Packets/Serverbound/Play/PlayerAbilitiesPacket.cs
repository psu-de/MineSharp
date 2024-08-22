using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.NetworkTypes;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Player abilities packet sent by the client to update the player's abilities.
/// </summary>
/// <param name="Flags">Bit mask indicating the player's abilities. Client may only send the <see cref="PlayerAbilitiesFlags.Flying"/> flag.</param>
public sealed partial record PlayerAbilitiesPacket(PlayerAbilitiesFlags Flags) : IPacketStatic<PlayerAbilitiesPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_Abilities;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteByte((byte)Flags);
    }

    /// <inheritdoc />
    public static PlayerAbilitiesPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var flags = (PlayerAbilitiesFlags)buffer.ReadByte();

        return new(flags);
    }
}
