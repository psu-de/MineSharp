using fNbt;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play
{
    /// <summary>
    ///     Represents the entity effect packet sent by the server to the client.
    /// </summary>
    /// <param name="EntityId">The entity ID</param>
    /// <param name="EffectId">The effect ID</param>
    /// <param name="Amplifier">The amplifier of the effect</param>
    /// <param name="Duration">The duration of the effect in ticks. -1 for infinity</param>
    /// <param name="Flags">The flags for the effect</param>
    /// <param name="HasFactorData">Indicates if the effect has factor data</param>
    /// <param name="FactorCodec">The factor codec data</param>
    public sealed record EntityEffectPacket(
        int EntityId,
        int EffectId,
        byte Amplifier,
        int Duration,
        EffectFlags Flags,
        bool HasFactorData,
        NbtTag? FactorCodec) : IPacket
    {
        /// <inheritdoc />
        public PacketType Type => StaticType;
        /// <inheritdoc />
        public static PacketType StaticType => PacketType.CB_Play_EntityEffect;

        /// <inheritdoc />
        public void Write(PacketBuffer buffer, MinecraftData version)
        {
            buffer.WriteVarInt(EntityId);
            buffer.WriteVarInt(EffectId);
            buffer.WriteByte(Amplifier);
            buffer.WriteVarInt(Duration);
            buffer.WriteByte((byte)Flags);
            buffer.WriteBool(HasFactorData);
            if (HasFactorData)
            {
                buffer.WriteNbt(FactorCodec!);
            }
        }

        /// <inheritdoc />
        public static IPacket Read(PacketBuffer buffer, MinecraftData version)
        {
            var entityId = buffer.ReadVarInt();
            var effectId = buffer.ReadVarInt();
            var amplifier = buffer.ReadByte();
            var duration = buffer.ReadVarInt();
            var flags = (EffectFlags)buffer.ReadByte();
            var hasFactorData = buffer.ReadBool();
            NbtTag? factorCodec = null;
            if (hasFactorData)
            {
                // TODO: Check if this is correct
                factorCodec = buffer.ReadNbt();
            }

            return new EntityEffectPacket(
                entityId,
                effectId,
                amplifier,
                duration,
                flags,
                hasFactorData,
                factorCodec);
        }
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    [Flags]
    public enum EffectFlags
    {
        None = 0,
        /// <summary>
        ///  Is ambient - was the effect spawned from a beacon?
        ///  All beacon-generated effects are ambient.
        ///  Ambient effects use a different icon in the HUD (blue border rather than gray).
        ///  If all effects on an entity are ambient, the "Is potion effect ambient" living metadata field should be set to true.
        ///  Usually should not be enabled.
        /// </summary>
        IsAmbient = 0x01,
        /// <summary>
        /// Show particles - should all particles from this effect be hidden?
        /// Effects with particles hidden are not included in the calculation of the effect color,
        /// and are not rendered on the HUD (but are still rendered within the inventory).
        /// Usually should be enabled.
        /// </summary>
        ShowParticles = 0x02,
        /// <summary>
        /// Show icon - should the icon be displayed on the client?
        /// Usually should be enabled.
        /// </summary>
        ShowIcon = 0x04
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
