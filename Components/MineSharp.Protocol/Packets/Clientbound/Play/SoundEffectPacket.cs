﻿using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public sealed partial record SoundEffectPacket(
    int SoundId,
    Identifier? SoundName,
    bool? HasFixedRange,
    float? Range,
    int SoundCategory,
    int EffectPositionX,
    int EffectPositionY,
    int EffectPositionZ,
    float Volume,
    float Pitch,
    long Seed
) : IPacketStatic<SoundEffectPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_SoundEffect;

    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(SoundId);
        if (SoundId == 0)
        {
            buffer.WriteIdentifier(SoundName!);
            buffer.WriteBool(HasFixedRange!.Value);
            if (HasFixedRange.Value)
            {
                buffer.WriteFloat(Range!.Value);
            }
        }
        buffer.WriteVarInt(SoundCategory);
        buffer.WriteInt(EffectPositionX);
        buffer.WriteInt(EffectPositionY);
        buffer.WriteInt(EffectPositionZ);
        buffer.WriteFloat(Volume);
        buffer.WriteFloat(Pitch);
        buffer.WriteLong(Seed);
    }

    public static SoundEffectPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var soundId = buffer.ReadVarInt();
        Identifier? soundName = null;
        bool? hasFixedRange = null;
        float? range = null;

        if (soundId == 0)
        {
            soundName = buffer.ReadIdentifier();
            hasFixedRange = buffer.ReadBool();
            if (hasFixedRange.Value)
            {
                range = buffer.ReadFloat();
            }
        }

        var soundCategory = buffer.ReadVarInt();
        var effectPositionX = buffer.ReadInt();
        var effectPositionY = buffer.ReadInt();
        var effectPositionZ = buffer.ReadInt();
        var volume = buffer.ReadFloat();
        var pitch = buffer.ReadFloat();
        var seed = buffer.ReadLong();

        return new SoundEffectPacket(soundId, soundName, hasFixedRange, range, soundCategory, effectPositionX, effectPositionY, effectPositionZ, volume, pitch, seed);
    }
}
#pragma warning restore CS1591
