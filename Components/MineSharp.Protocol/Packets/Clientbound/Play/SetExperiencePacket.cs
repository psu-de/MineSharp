using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Set Experience packet
/// </summary>
public abstract partial record SetExperiencePacket : IPacketStatic<SetExperiencePacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_Experience;

    // all versions contain these fields:
    /// <summary>
    /// The experience bar value between 0 and 1
    /// </summary>
    public abstract float ExperienceBar { get; init; }
    /// <summary>
    /// The experience level
    /// </summary>
    public abstract int Level { get; init; }
    /// <summary>
    /// The total experience points
    /// </summary>
    public abstract int TotalExperience { get; init; }

    // may only be called from sub class in this class
    private SetExperiencePacket()
    {
    }

    /// <summary>
    /// Version specific <see cref="SetExperiencePacket"/> for <see cref="Core.ProtocolVersion.V_1_7_0"/>
    /// </summary>
    public sealed partial record SetExperiencePacketV_1_7_0(float ExperienceBar, short ShortLevel, short ShortTotalExperience) : SetExperiencePacket, IPacketVersionSubTypeStatic<SetExperiencePacketV_1_7_0, SetExperiencePacket>
    {
        /// <inheritdoc />
        public override int Level
        {
            get
            {
                return ShortLevel;
            }
            init
            {
                ShortLevel = (short)value;
            }
        }
        /// <inheritdoc />
        public override int TotalExperience
        {
            get
            {
                return ShortTotalExperience;
            }
            init
            {
                ShortTotalExperience = (short)value;
            }
        }

        /// <inheritdoc />
        public override void Write(PacketBuffer buffer, MinecraftData data)
        {
            buffer.WriteFloat(ExperienceBar);
            buffer.WriteShort(ShortLevel);
            buffer.WriteShort(ShortTotalExperience);
        }

        /// <inheritdoc />
        public static new SetExperiencePacketV_1_7_0 Read(PacketBuffer buffer, MinecraftData data)
        {
            var experienceBar = buffer.ReadFloat();
            var level = buffer.ReadShort();
            var totalExperience = buffer.ReadShort();

            return new(experienceBar, level, totalExperience);
        }
    }

    /// <summary>
    /// Version specific <see cref="SetExperiencePacket"/> for <see cref="Core.ProtocolVersion.V_1_8_0"/>.
    /// 
    /// Level and TotalExperience become VarInt in 1.8.0.
    /// </summary>
    public sealed partial record SetExperiencePacketV_1_8_0(float ExperienceBar, int Level, int TotalExperience) : SetExperiencePacket, IPacketVersionSubTypeStatic<SetExperiencePacketV_1_8_0, SetExperiencePacket>
    {
        /// <inheritdoc />
        public override void Write(PacketBuffer buffer, MinecraftData data)
        {
            buffer.WriteFloat(ExperienceBar);
            buffer.WriteVarInt(Level);
            buffer.WriteVarInt(TotalExperience);
        }

        /// <inheritdoc />
        public static new SetExperiencePacketV_1_8_0 Read(PacketBuffer buffer, MinecraftData data)
        {
            var experienceBar = buffer.ReadFloat();
            var level = buffer.ReadVarInt();
            var totalExperience = buffer.ReadVarInt();

            return new(experienceBar, level, totalExperience);
        }
    }

    /// <summary>
    /// Version specific <see cref="SetExperiencePacket"/> for <see cref="Core.ProtocolVersion.V_1_19_3"/>.
    /// 
    /// Level and TotalExperience are swapped in 1.19.3.
    /// </summary>
    public sealed partial record SetExperiencePacketV_1_19_3(float ExperienceBar, int Level, int TotalExperience) : SetExperiencePacket, IPacketVersionSubTypeStatic<SetExperiencePacketV_1_19_3, SetExperiencePacket>
    {
        /// <inheritdoc />
        public override void Write(PacketBuffer buffer, MinecraftData data)
        {
            buffer.WriteFloat(ExperienceBar);
            buffer.WriteVarInt(TotalExperience);
            buffer.WriteVarInt(Level);
        }

        /// <inheritdoc />
        public static new SetExperiencePacketV_1_19_3 Read(PacketBuffer buffer, MinecraftData data)
        {
            var experienceBar = buffer.ReadFloat();
            var totalExperience = buffer.ReadVarInt();
            var level = buffer.ReadVarInt();

            return new(experienceBar, level, totalExperience);
        }
    }

    /// <summary>
    /// Version specific <see cref="SetExperiencePacket"/> for <see cref="Core.ProtocolVersion.V_1_20_2"/>.
    /// 
    /// Level and TotalExperience are swapped back again in 1.20.2.
    /// </summary>
    public sealed partial record SetExperiencePacketV_1_20_2(float ExperienceBar, int Level, int TotalExperience) : SetExperiencePacket, IPacketVersionSubTypeStatic<SetExperiencePacketV_1_20_2, SetExperiencePacket>
    {
        /// <inheritdoc />
        public override void Write(PacketBuffer buffer, MinecraftData data)
        {
            buffer.WriteFloat(ExperienceBar);
            buffer.WriteVarInt(Level);
            buffer.WriteVarInt(TotalExperience);
        }

        /// <inheritdoc />
        public static new SetExperiencePacketV_1_20_2 Read(PacketBuffer buffer, MinecraftData data)
        {
            var experienceBar = buffer.ReadFloat();
            var level = buffer.ReadVarInt();
            var totalExperience = buffer.ReadVarInt();

            return new(experienceBar, level, totalExperience);
        }
    }
}
