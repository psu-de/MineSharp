using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class SoundEffectPacket : IPacket
{
    public SoundEffectPacket(int soundId, string? soundName, bool? hasFixedRange, float? range, int soundCategory, int effectPositionX, int effectPositionY, int effectPositionZ, float volume, float pitch, long seed)
    {
        SoundId = soundId;
        SoundName = soundName;
        HasFixedRange = hasFixedRange;
        Range = range;
        SoundCategory = soundCategory;
        EffectPositionX = effectPositionX;
        EffectPositionY = effectPositionY;
        EffectPositionZ = effectPositionZ;
        Volume = volume;
        Pitch = pitch;
        Seed = seed;
    }

    public int SoundId { get; set; }
    public string? SoundName { get; set; }
    public bool? HasFixedRange { get; set; }
    public float? Range { get; set; }
    public int SoundCategory { get; set; }
    public int EffectPositionX { get; set; }
    public int EffectPositionY { get; set; }
    public int EffectPositionZ { get; set; }
    public float Volume { get; set; }
    public float Pitch { get; set; }
    public long Seed { get; set; }
    public PacketType Type => PacketType.CB_Play_SoundEffect;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(SoundId);
        if (SoundId == 0)
        {
            buffer.WriteString(SoundName!);
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

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var soundId = buffer.ReadVarInt();
        string? soundName = null;
        bool? hasFixedRange = null;
        float? range = null;

        if (soundId == 0)
        {
            soundName = buffer.ReadString();
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
