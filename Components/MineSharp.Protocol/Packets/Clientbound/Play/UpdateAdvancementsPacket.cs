using MineSharp.ChatComponent;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Items;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.NetworkTypes;
using static MineSharp.Protocol.Packets.Clientbound.Play.UpdateAdvancementsPacket;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Update Advancements packet
/// </summary>
/// <param name="ResetClear">Whether to reset/clear the current advancements</param>
/// <param name="AdvancementMappings">The advancement mappings</param>
/// <param name="Identifiers">The identifiers of the advancements that should be removed</param>
/// <param name="ProgressMappings">The progress mappings</param>
public sealed record UpdateAdvancementsPacket(
    bool ResetClear,
    KeyValuePair<Identifier, Advancement>[] AdvancementMappings,
    Identifier[] Identifiers,
    KeyValuePair<Identifier, AdvancementProgress>[] ProgressMappings) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_Advancements;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteBool(ResetClear);
        buffer.WriteVarInt(AdvancementMappings.Length);
        foreach (var (key, value) in AdvancementMappings)
        {
            buffer.WriteIdentifier(key);
            value.Write(buffer, data);
        }
        buffer.WriteVarInt(Identifiers.Length);
        foreach (var identifier in Identifiers)
        {
            buffer.WriteIdentifier(identifier);
        }
        buffer.WriteVarInt(ProgressMappings.Length);
        foreach (var (key, value) in ProgressMappings)
        {
            buffer.WriteIdentifier(key);
            value.Write(buffer);
        }
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var resetClear = buffer.ReadBool();
        var advancementMappingsCount = buffer.ReadVarInt();
        var advancementMappings = new KeyValuePair<Identifier, Advancement>[advancementMappingsCount];
        for (int i = 0; i < advancementMappingsCount; i++)
        {
            var key = buffer.ReadIdentifier();
            var value = Advancement.Read(buffer, data);
            advancementMappings[i] = new(key, value);
        }
        var identifiersCount = buffer.ReadVarInt();
        var identifiers = new Identifier[identifiersCount];
        for (int i = 0; i < identifiersCount; i++)
        {
            identifiers[i] = buffer.ReadIdentifier();
        }
        var progressMappingsCount = buffer.ReadVarInt();
        var progressMappings = new KeyValuePair<Identifier, AdvancementProgress>[progressMappingsCount];
        for (int i = 0; i < progressMappingsCount; i++)
        {
            var key = buffer.ReadIdentifier();
            var value = AdvancementProgress.Read(buffer);
            progressMappings[i] = new(key, value);
        }

        return new UpdateAdvancementsPacket(
            resetClear,
            advancementMappings,
            identifiers,
            progressMappings);
    }

    /// <summary>
    ///     Represents an advancement
    /// </summary>
    /// <param name="ParentId">The identifier of the parent advancement</param>
    /// <param name="DisplayData">The display data</param>
    /// <param name="Requirements">The requirements</param>
    /// <param name="SendsTelemetryData">Whether the client should include this achievement in the telemetry data when it's completed</param>
    public sealed record Advancement(
        Identifier? ParentId,
        AdvancementDisplay? DisplayData,
        string[][] Requirements,
        bool SendsTelemetryData) : ISerializableWithMinecraftData<Advancement>
    {
        /// <inheritdoc />
        public void Write(PacketBuffer buffer, MinecraftData data)
        {
            var hasParent = ParentId != null;
            buffer.WriteBool(hasParent);
            if (hasParent)
            {
                buffer.WriteIdentifier(ParentId!);
            }
            var hasDisplay = DisplayData != null;
            buffer.WriteBool(hasDisplay);
            if (hasDisplay)
            {
                DisplayData!.Write(buffer, data);
            }
            buffer.WriteVarInt(Requirements.Length);
            foreach (var requirement in Requirements)
            {
                buffer.WriteVarInt(requirement.Length);
                foreach (var req in requirement)
                {
                    buffer.WriteString(req);
                }
            }
            buffer.WriteBool(SendsTelemetryData);
        }

        /// <inheritdoc />
        public static Advancement Read(PacketBuffer buffer, MinecraftData data)
        {
            var hasParent = buffer.ReadBool();
            Identifier? parentId = null;
            if (hasParent)
            {
                parentId = buffer.ReadIdentifier();
            }
            var hasDisplay = buffer.ReadBool();
            AdvancementDisplay? displayData = null;
            if (hasDisplay)
            {
                displayData = AdvancementDisplay.Read(buffer, data);
            }
            var requirementsCount = buffer.ReadVarInt();
            var requirements = new string[requirementsCount][];
            for (int i = 0; i < requirementsCount; i++)
            {
                var innerCount = buffer.ReadVarInt();
                var innerList = new string[innerCount];
                for (int j = 0; j < innerCount; j++)
                {
                    innerList[j] = buffer.ReadString();
                }
                requirements[i] = innerList;
            }
            var sendsTelemetryData = buffer.ReadBool();

            return new Advancement(
                parentId,
                displayData,
                requirements,
                sendsTelemetryData);
        }
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public enum FrameType
    {
        Task = 0,
        Challenge = 1,
        Goal = 2
    }

    [Flags]
    public enum AdvancementFlags
    {
        None = 0,
        HasBackgroundTexture = 0x01,
        ShowToast = 0x02,
        Hidden = 0x04
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

    /// <summary>
    ///     Represents the display data of an advancement
    /// </summary>
    /// <param name="Title">The title of the advancement</param>
    /// <param name="Description">The description of the advancement</param>
    /// <param name="Icon">The icon of the advancement</param>
    /// <param name="FrameType">The frame type of the advancement</param>
    /// <param name="Flags">The flags of the advancement</param>
    /// <param name="BackgroundTexture">The background texture location</param>
    /// <param name="XCoord">The X coordinate of the advancement</param>
    /// <param name="YCoord">The Y coordinate of the advancement</param>
    public sealed record AdvancementDisplay(
        Chat Title,
        Chat Description,
        Item Icon,
        FrameType FrameType,
        AdvancementFlags Flags,
        Identifier? BackgroundTexture,
        float XCoord,
        float YCoord) : ISerializableWithMinecraftData<AdvancementDisplay>
    {
        /// <inheritdoc />
        public void Write(PacketBuffer buffer, MinecraftData data)
        {
            buffer.WriteChatComponent(Title);
            buffer.WriteChatComponent(Description);
            buffer.WriteOptionalItem(Icon);
            buffer.WriteVarInt((int)FrameType);
            buffer.WriteInt((int)Flags);
            if (Flags.HasFlag(AdvancementFlags.HasBackgroundTexture))
            {
                buffer.WriteIdentifier(BackgroundTexture ?? throw new InvalidOperationException($"{nameof(BackgroundTexture)} must not be null if {nameof(Flags)} indicates that the background texture is present."));
            }
            buffer.WriteFloat(XCoord);
            buffer.WriteFloat(YCoord);
        }

        /// <inheritdoc />
        public static AdvancementDisplay Read(PacketBuffer buffer, MinecraftData data)
        {
            var title = buffer.ReadChatComponent();
            var description = buffer.ReadChatComponent();
            var icon = buffer.ReadOptionalItem(data)!;
            var frameType = (FrameType)buffer.ReadVarInt();
            var flags = (AdvancementFlags)buffer.ReadInt();
            Identifier? backgroundTexture = null;
            if (flags.HasFlag(AdvancementFlags.HasBackgroundTexture))
            {
                backgroundTexture = buffer.ReadIdentifier();
            }
            var xCoord = buffer.ReadFloat();
            var yCoord = buffer.ReadFloat();

            return new AdvancementDisplay(
                title,
                description,
                icon,
                frameType,
                flags,
                backgroundTexture,
                xCoord,
                yCoord);
        }
    }

    /// <summary>
    ///     Represents the progress of an advancement
    /// </summary>
    /// <param name="Criteria">The criteria of the advancement progress</param>
    public sealed record AdvancementProgress(
        KeyValuePair<Identifier, CriterionProgress>[] Criteria) : ISerializable<AdvancementProgress>
    {
        /// <inheritdoc />
        public void Write(PacketBuffer buffer)
        {
            buffer.WriteVarInt(Criteria.Length);
            foreach (var (key, value) in Criteria)
            {
                buffer.WriteIdentifier(key);
                value.Write(buffer);
            }
        }

        /// <inheritdoc />
        public static AdvancementProgress Read(PacketBuffer buffer)
        {
            var criteriaCount = buffer.ReadVarInt();
            var criteria = new KeyValuePair<Identifier, CriterionProgress>[criteriaCount];
            for (int i = 0; i < criteriaCount; i++)
            {
                var key = buffer.ReadIdentifier();
                var value = CriterionProgress.Read(buffer);
                criteria[i] = new(key, value);
            }

            return new AdvancementProgress(criteria);
        }
    }

    /// <summary>
    ///     Represents the progress of a criterion
    /// </summary>
    /// <param name="DateOfAchieving">The date of achieving the criterion</param>
    public sealed record CriterionProgress(
        long? DateOfAchieving) : ISerializable<CriterionProgress>
    {
        /// <inheritdoc />
        public void Write(PacketBuffer buffer)
        {
            var achieved = DateOfAchieving != null;
            buffer.WriteBool(achieved);
            if (achieved)
            {
                buffer.WriteLong(DateOfAchieving!.Value);
            }
        }

        /// <inheritdoc />
        public static CriterionProgress Read(PacketBuffer buffer)
        {
            var achieved = buffer.ReadBool();
            long? dateOfAchieving = null;
            if (achieved)
            {
                dateOfAchieving = buffer.ReadLong();
            }

            return new CriterionProgress(
                dateOfAchieving);
        }
    }
}
