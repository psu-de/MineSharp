using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Set Experience packet
/// </summary>
/// <param name="ExperienceBar">The experience bar value between 0 and 1</param>
/// <param name="Level">The experience level</param>
/// <param name="TotalExperience">The total experience points</param>
public sealed record SetExperiencePacket(float ExperienceBar, int Level, int TotalExperience) : IPacketStatic<SetExperiencePacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_Experience;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteFloat(ExperienceBar);
        buffer.WriteVarInt(Level);
        buffer.WriteVarInt(TotalExperience);
    }

    /// <inheritdoc />
    public static SetExperiencePacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var experienceBar = buffer.ReadFloat();
        var level = buffer.ReadVarInt();
        var totalExperience = buffer.ReadVarInt();

        return new SetExperiencePacket(experienceBar, level, totalExperience);
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
