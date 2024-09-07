using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Changes the effect of the current beacon.
/// </summary>
/// <param name="PrimaryEffect">The primary effect ID</param>
/// <param name="SecondaryEffect">The secondary effect ID</param>
public sealed record SetBeaconEffectPacket(int? PrimaryEffect, int? SecondaryEffect) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_SetBeaconEffect;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        var hasPrimaryEffect = PrimaryEffect.HasValue;
        buffer.WriteBool(hasPrimaryEffect);
        if (hasPrimaryEffect)
        {
            buffer.WriteVarInt(PrimaryEffect!.Value);
        }

        var hasSecondaryEffect = SecondaryEffect.HasValue;
        buffer.WriteBool(hasSecondaryEffect);
        if (hasSecondaryEffect)
        {
            buffer.WriteVarInt(SecondaryEffect!.Value);
        }
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var hasPrimaryEffect = buffer.ReadBool();
        int? primaryEffect = hasPrimaryEffect ? buffer.ReadVarInt() : null;

        var hasSecondaryEffect = buffer.ReadBool();
        int? secondaryEffect = hasSecondaryEffect ? buffer.ReadVarInt() : null;

        return new SetBeaconEffectPacket(
            primaryEffect,
            secondaryEffect);
    }
}
