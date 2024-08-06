using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class EntitySoundEffectPacket : IPacket
{
    public EntitySoundEffectPacket(int soundId, string? soundName, bool? hasFixedRange, float? range, int soundCategory, int entityId, float volume, float pitch, long seed)
    {
        SoundId = soundId;
        SoundName = soundName;
        HasFixedRange = hasFixedRange;
        Range = range;
        SoundCategory = soundCategory;
        EntityId = entityId;
        Volume = volume;
        Pitch = pitch;
        Seed = seed;
    }

    public int SoundId { get; set; }
    public string? SoundName { get; set; }
    public bool? HasFixedRange { get; set; }
    public float? Range { get; set; }
    public int SoundCategory { get; set; }
    public int EntityId { get; set; }
    public float Volume { get; set; }
    public float Pitch { get; set; }
    public long Seed { get; set; }
    public PacketType Type => PacketType.CB_Play_EntitySoundEffect;

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
        buffer.WriteVarInt(EntityId);
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
        var entityId = buffer.ReadVarInt();
        var volume = buffer.ReadFloat();
        var pitch = buffer.ReadFloat();
        var seed = buffer.ReadLong();

        return new EntitySoundEffectPacket(soundId, soundName, hasFixedRange, range, soundCategory, entityId, volume, pitch, seed);
    }
}
#pragma warning restore CS1591
