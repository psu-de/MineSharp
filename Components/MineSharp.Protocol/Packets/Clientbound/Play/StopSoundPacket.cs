using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using static MineSharp.Protocol.Packets.Clientbound.Play.StopSoundPacket;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Stop Sound packet
/// </summary>
/// <param name="Category">Optional category of the sound. If not present, then sounds from all sources are cleared.</param>
/// <param name="Sound">Optional sound effect name. If not present, then all sounds are cleared.</param>
public sealed record StopSoundPacket(SoundCategory? Category, Identifier? Sound) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_StopSound;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        byte flags = 0;
        flags |= (byte)(Category.HasValue ? 0x1 : 0);
        flags |= (byte)(Sound != null ? 0x2 : 0);

        buffer.WriteByte(flags);
        if (Category.HasValue)
        {
            buffer.WriteVarInt((int)Category.GetValueOrDefault());
        }
        if (Sound != null)
        {
            buffer.WriteIdentifier(Sound);
        }
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var flags = buffer.ReadByte();
        SoundCategory? category = null;
        Identifier? sound = null;

        if ((flags & 0x1) != 0)
        {
            category = (SoundCategory)buffer.ReadVarInt();
        }
        if ((flags & 0x2) != 0)
        {
            sound = buffer.ReadIdentifier();
        }

        return new StopSoundPacket(category, sound);
    }

	/// <summary>
	///     Enum representing sound categories
	/// </summary>
	public enum SoundCategory
	{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Master = 0,
        Music = 1,
		Record = 2,
		Weather = 3,
		Block = 4,
		Hostile = 5,
		Neutral = 6,
		Player = 7,
		Ambient = 8,
		Voice = 9
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
	}
}
