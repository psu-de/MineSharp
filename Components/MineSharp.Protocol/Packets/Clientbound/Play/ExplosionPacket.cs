using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.NetworkTypes;
using static MineSharp.Protocol.Packets.Clientbound.Play.ExplosionPacket;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Explosion packet sent when an explosion occurs (creepers, TNT, and ghast fireballs).
/// </summary>
/// <param name="X">The X coordinate of the explosion.</param>
/// <param name="Y">The Y coordinate of the explosion.</param>
/// <param name="Z">The Z coordinate of the explosion.</param>
/// <param name="Strength">The strength of the explosion.</param>
/// <param name="Records">The affected block records.</param>
/// <param name="PlayerMotionX">The X velocity of the player being pushed by the explosion.</param>
/// <param name="PlayerMotionY">The Y velocity of the player being pushed by the explosion.</param>
/// <param name="PlayerMotionZ">The Z velocity of the player being pushed by the explosion.</param>
/// <param name="BlockInteraction">The block interaction type.</param>
/// <param name="SmallExplosionParticleID">The particle ID for small explosion particles.</param>
/// <param name="SmallExplosionParticleData">The particle data for small explosion particles.</param>
/// <param name="LargeExplosionParticleID">The particle ID for large explosion particles.</param>
/// <param name="LargeExplosionParticleData">The particle data for large explosion particles.</param>
/// <param name="Sound">The sound played during the explosion.</param>
public sealed record ExplosionPacket(
    double X,
    double Y,
    double Z,
    float Strength,
    (byte X, byte Y, byte Z)[] Records,
    float PlayerMotionX,
    float PlayerMotionY,
    float PlayerMotionZ,
    BlockInteractionType BlockInteraction,
    int SmallExplosionParticleID,
    IParticleData? SmallExplosionParticleData,
    int LargeExplosionParticleID,
    IParticleData? LargeExplosionParticleData,
    ExplosionSound Sound
) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_Explosion;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteDouble(X);
        buffer.WriteDouble(Y);
        buffer.WriteDouble(Z);
        buffer.WriteFloat(Strength);
        buffer.WriteVarInt(Records.Length);
        foreach (var record in Records)
        {
            buffer.WriteByte(record.X);
            buffer.WriteByte(record.Y);
            buffer.WriteByte(record.Z);
        }
        buffer.WriteFloat(PlayerMotionX);
        buffer.WriteFloat(PlayerMotionY);
        buffer.WriteFloat(PlayerMotionZ);
        buffer.WriteVarInt((int)BlockInteraction);
        buffer.WriteVarInt(SmallExplosionParticleID);
        SmallExplosionParticleData?.Write(buffer, data);
        buffer.WriteVarInt(LargeExplosionParticleID);
        LargeExplosionParticleData?.Write(buffer, data);
        Sound.Write(buffer);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var x = buffer.ReadDouble();
        var y = buffer.ReadDouble();
        var z = buffer.ReadDouble();
        var strength = buffer.ReadFloat();
        var recordCount = buffer.ReadVarInt();
        var records = new (byte X, byte Y, byte Z)[recordCount];
        for (int i = 0; i < recordCount; i++)
        {
            records[i] = (buffer.ReadByte(), buffer.ReadByte(), buffer.ReadByte());
        }
        var playerMotionX = buffer.ReadFloat();
        var playerMotionY = buffer.ReadFloat();
        var playerMotionZ = buffer.ReadFloat();
        var blockInteraction = (BlockInteractionType)buffer.ReadVarInt();
        var smallExplosionParticleID = buffer.ReadVarInt();
        var smallExplosionParticleData = ParticleDataRegistry.Read(buffer, data, smallExplosionParticleID);
        var largeExplosionParticleID = buffer.ReadVarInt();
        var largeExplosionParticleData = ParticleDataRegistry.Read(buffer, data, largeExplosionParticleID);
        var sound = ExplosionSound.Read(buffer);

        return new ExplosionPacket(
            x, y, z, strength, records, playerMotionX, playerMotionY, playerMotionZ,
            blockInteraction, smallExplosionParticleID, smallExplosionParticleData,
            largeExplosionParticleID, largeExplosionParticleData, sound
        );
    }

    /// <summary>
    ///     Represents the sound played during an explosion.
    /// </summary>
    /// <param name="SoundName">The name of the sound played.</param>
    /// <param name="Range">The fixed range of the sound.</param>
    public sealed record ExplosionSound(
        Identifier SoundName,
        float? Range
    ) : ISerializable<ExplosionSound>
    {
        /// <inheritdoc />
        public void Write(PacketBuffer buffer)
        {
            buffer.WriteIdentifier(SoundName);
            var hasFixedRange = Range.HasValue;
            buffer.WriteBool(hasFixedRange);
            if (hasFixedRange)
            {
                buffer.WriteFloat(Range!.Value);
            }
        }

        /// <inheritdoc />
        public static ExplosionSound Read(PacketBuffer buffer)
        {
            var soundName = buffer.ReadIdentifier();
            var hasFixedRange = buffer.ReadBool();
            float? range = hasFixedRange ? buffer.ReadFloat() : null;

            return new ExplosionSound(soundName, range);
        }
    }

    /// <summary>
    ///     Enum representing the block interaction type during an explosion.
    /// </summary>
    public enum BlockInteractionType
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Keep = 0,
        Destroy = 1,
        DestroyWithDecay = 2,
        TriggerBlock = 3
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
