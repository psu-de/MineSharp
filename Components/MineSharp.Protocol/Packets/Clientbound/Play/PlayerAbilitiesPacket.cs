using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using static MineSharp.Protocol.Packets.Clientbound.Play.PlayerAbilitiesPacket;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Player abilities packet sent by the server to update the player's abilities.
/// </summary>
/// <param name="Flags">Bit field indicating various abilities.</param>
/// <param name="FlyingSpeed">The flying speed of the player.</param>
/// <param name="FieldOfViewModifier">Modifies the field of view, like a speed potion.</param>
public sealed record PlayerAbilitiesPacket(PlayerAbilitiesFlags Flags, float FlyingSpeed, float FieldOfViewModifier) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_Abilities;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteByte((byte)Flags);
        buffer.WriteFloat(FlyingSpeed);
        buffer.WriteFloat(FieldOfViewModifier);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var flags = (PlayerAbilitiesFlags)buffer.ReadByte();
        var flyingSpeed = buffer.ReadByte();
        var fieldOfViewModifier = buffer.ReadByte();

        return new PlayerAbilitiesPacket(flags, flyingSpeed, fieldOfViewModifier);
    }

    /// <summary>
    /// Flags representing various player abilities.
    /// </summary>
    [Flags]
    public enum PlayerAbilitiesFlags : byte
    {
        /// <summary>
        /// Player is invulnerable.
        /// </summary>
        Invulnerable = 0x01,
        /// <summary>
        /// Player is flying.
        /// </summary>
        Flying = 0x02,
        /// <summary>
        /// Player is allowed to fly.
        /// </summary>
        AllowFlying = 0x04,
        /// <summary>
        /// Player is in creative mode.
        /// And can instantly break blocks.
        /// </summary>
        CreativeMode = 0x08,
    }
}
