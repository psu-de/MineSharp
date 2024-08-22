using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.NetworkTypes;

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
        buffer.WriteSByte((sbyte)Flags);
        buffer.WriteFloat(FlyingSpeed);
        buffer.WriteFloat(FieldOfViewModifier);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var flags = (PlayerAbilitiesFlags)buffer.ReadSByte();
        var flyingSpeed = buffer.ReadFloat();
        var fieldOfViewModifier = buffer.ReadFloat();

        return new PlayerAbilitiesPacket(flags, flyingSpeed, fieldOfViewModifier);
    }
}
