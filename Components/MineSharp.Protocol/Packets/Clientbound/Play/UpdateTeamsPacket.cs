using System.Collections.Frozen;
using MineSharp.ChatComponent;
using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using static MineSharp.Protocol.Packets.Clientbound.Play.UpdateTeamsPacket;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Update Teams packet
/// </summary>
/// <param name="TeamName">A unique name for the team</param>
/// <param name="MethodData">The data for the method type of this packet</param>
public sealed record UpdateTeamsPacket(string TeamName, IUpdateTeamsMethod MethodData) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_Teams;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteString(TeamName);
        buffer.WriteByte((byte)MethodData.MethodType);
        MethodData.Write(buffer);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var teamName = buffer.ReadString();
        var method = (UpdateTeamsMethodType)buffer.ReadByte();
        var methodData = UpdateTeamsMethodRegistry.Read(buffer, method);
        return new UpdateTeamsPacket(teamName, methodData);
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public enum UpdateTeamsMethodType
    {
        CreateTeam = 0,
        RemoveTeam = 1,
        UpdateTeamInfo = 2,
        AddEntitiesToTeam = 3,
        RemoveEntitiesFromTeam = 4
    }

    public interface IUpdateTeamsMethod
    {
        public UpdateTeamsMethodType MethodType { get; }

        public void Write(PacketBuffer buffer);
    }

    public interface IUpdateTeamsMethodStatic
    {
        public static abstract UpdateTeamsMethodType StaticMethodType { get; }

        public static abstract IUpdateTeamsMethod Read(PacketBuffer buffer);
    }

    public static class UpdateTeamsMethodRegistry
    {
        public static IUpdateTeamsMethod Read(PacketBuffer buffer, UpdateTeamsMethodType method)
        {
            if (!UpdateTeamsMethodFactories.TryGetValue(method, out var reader))
            {
                throw new InvalidOperationException($"Unsupported UpdateTeamsMethodType: {method}");
            }
            return reader(buffer);
        }

        public static readonly FrozenDictionary<UpdateTeamsMethodType, Func<PacketBuffer, IUpdateTeamsMethod>> UpdateTeamsMethodFactories;

        static UpdateTeamsMethodRegistry()
        {
            UpdateTeamsMethodFactories = InitializeUpdateTeamsMethod();
        }

        private static FrozenDictionary<UpdateTeamsMethodType, Func<PacketBuffer, IUpdateTeamsMethod>> InitializeUpdateTeamsMethod()
        {
            var dict = new Dictionary<UpdateTeamsMethodType, Func<PacketBuffer, IUpdateTeamsMethod>>();

            void Register<T>()
                where T : IUpdateTeamsMethod, IUpdateTeamsMethodStatic
            {
                var mask = T.StaticMethodType;
                var factory = T.Read;
                dict.Add(mask, factory);
            }

            Register<CreateTeam>();
            Register<RemoveTeam>();
            Register<UpdateTeamInfo>();
            Register<AddEntitiesToTeam>();
            Register<RemoveEntitiesFromTeam>();

            return dict.ToFrozenDictionary();
        }
    }

    #region Parts

    /// <summary>
    /// Marker interface for the parts of the UpdateTeams packet.
    /// </summary>
    public interface IUpdateTeamsPart
    {
    }

    public sealed record UpdateTeamsTeamInfoPart(
        Chat TeamDisplayName,
        byte FriendlyFlags,
        NameTagVisibility NameTagVisibility,
        CollisionRule CollisionRule,
        TextColor TeamColor,
        Chat TeamPrefix,
        Chat TeamSuffix
    ) : IUpdateTeamsPart, ISerializable<UpdateTeamsTeamInfoPart>
    {
        public void Write(PacketBuffer buffer)
        {
            buffer.WriteChatComponent(TeamDisplayName);
            buffer.WriteByte(FriendlyFlags);
            buffer.WriteString(NameTagVisibilityExtensions.ToUpdateTeamPacketString(NameTagVisibility));
            buffer.WriteString(CollisionRuleExtensions.ToUpdateTeamPacketString(CollisionRule));
            buffer.WriteVarInt((int)TeamColor);
            buffer.WriteChatComponent(TeamPrefix);
            buffer.WriteChatComponent(TeamSuffix);
        }

        public static UpdateTeamsTeamInfoPart Read(PacketBuffer buffer)
        {
            var teamDisplayName = buffer.ReadChatComponent();
            var friendlyFlags = buffer.ReadByte();
            var nameTagVisibility = NameTagVisibilityExtensions.FromUpdateTeamPacketString(buffer.ReadString());
            var collisionRule = CollisionRuleExtensions.FromUpdateTeamPacketString(buffer.ReadString());
            var teamColor = (TextColor)buffer.ReadVarInt();
            var teamPrefix = buffer.ReadChatComponent();
            var teamSuffix = buffer.ReadChatComponent();
            return new UpdateTeamsTeamInfoPart(teamDisplayName, friendlyFlags, nameTagVisibility, collisionRule, teamColor, teamPrefix, teamSuffix);
        }
    }

    /// <param name="Entities">Identifiers entities. For players, this is their username; for other entities, it is their UUID.</param>
    public sealed record UpdateTeamsEntityInfoPart(
        string[] Entities
    ) : IUpdateTeamsPart, ISerializable<UpdateTeamsEntityInfoPart>
    {
        public void Write(PacketBuffer buffer)
        {
            buffer.WriteVarIntArray(Entities, (buffer, entity) => buffer.WriteString(entity));
        }

        public static UpdateTeamsEntityInfoPart Read(PacketBuffer buffer)
        {
            var entities = buffer.ReadVarIntArray((buffer) => buffer.ReadString());
            return new UpdateTeamsEntityInfoPart(entities);
        }
    }

    /// <summary>
    /// Enum representing the visibility options for name tags in Minecraft.
    /// </summary>
    public enum NameTagVisibility
    {
        Always,
        HideForOtherTeams,
        HideForOwnTeam,
        Never
    }

    public static class NameTagVisibilityExtensions
    {
        private static readonly FrozenDictionary<NameTagVisibility, string> EnumToString = new Dictionary<NameTagVisibility, string>()
        {
            { NameTagVisibility.Always, "always" },
            { NameTagVisibility.HideForOtherTeams, "hideForOtherTeams" },
            { NameTagVisibility.HideForOwnTeam, "hideForOwnTeam" },
            { NameTagVisibility.Never, "never" }
        }.ToFrozenDictionary();

        private static readonly FrozenDictionary<string, NameTagVisibility> StringToEnum = EnumToString
            .ToDictionary(kvp => kvp.Value, kvp => kvp.Key).ToFrozenDictionary();

        // can't be an extension method because this class is not top level
        public static string ToUpdateTeamPacketString(NameTagVisibility visibility)
        {
            return EnumToString[visibility];
        }

        public static NameTagVisibility FromUpdateTeamPacketString(string visibility)
        {
            if (StringToEnum.TryGetValue(visibility, out var enumValue))
            {
                return enumValue;
            }
            throw new ArgumentException($"Invalid visibility string: {visibility}", nameof(visibility));
        }
    }

    /// <summary>
    /// Enum representing the collision rules in Minecraft.
    /// </summary>
    public enum CollisionRule
    {
        Always,
        PushOtherTeams,
        PushOwnTeam,
        Never
    }

    public static class CollisionRuleExtensions
    {
        private static readonly FrozenDictionary<CollisionRule, string> EnumToString = new Dictionary<CollisionRule, string>()
        {
            { CollisionRule.Always, "always" },
            { CollisionRule.PushOtherTeams, "pushOtherTeams" },
            { CollisionRule.PushOwnTeam, "pushOwnTeam" },
            { CollisionRule.Never, "never" }
        }.ToFrozenDictionary();

        private static readonly FrozenDictionary<string, CollisionRule> StringToEnum = EnumToString
            .ToDictionary(kvp => kvp.Value, kvp => kvp.Key).ToFrozenDictionary();

        // can't be an extension method because this class is not top level
        public static string ToUpdateTeamPacketString(CollisionRule rule)
        {
            return EnumToString[rule];
        }

        public static CollisionRule FromUpdateTeamPacketString(string rule)
        {
            if (StringToEnum.TryGetValue(rule, out var enumValue))
            {
                return enumValue;
            }
            throw new ArgumentException($"Invalid collision rule string: {rule}", nameof(rule));
        }
    }

    #endregion

    public sealed record CreateTeam(
        UpdateTeamsTeamInfoPart TeamInfoPart,
        UpdateTeamsEntityInfoPart EntityInfoPart
    ) : IUpdateTeamsMethod, IUpdateTeamsMethodStatic, ISerializable<CreateTeam>
    {
        public UpdateTeamsMethodType MethodType => StaticMethodType;
        public static UpdateTeamsMethodType StaticMethodType => UpdateTeamsMethodType.CreateTeam;

        public void Write(PacketBuffer buffer)
        {
            TeamInfoPart.Write(buffer);
            EntityInfoPart.Write(buffer);
        }

        public static CreateTeam Read(PacketBuffer buffer)
        {
            var teamInfoPart = UpdateTeamsTeamInfoPart.Read(buffer);
            var entityInfoPart = UpdateTeamsEntityInfoPart.Read(buffer);
            return new(teamInfoPart, entityInfoPart);
        }

        static IUpdateTeamsMethod IUpdateTeamsMethodStatic.Read(PacketBuffer buffer)
        {
            return Read(buffer);
        }
    }

    public sealed record RemoveTeam()
        : IUpdateTeamsMethod, IUpdateTeamsMethodStatic, ISerializable<RemoveTeam>
    {
        public UpdateTeamsMethodType MethodType => StaticMethodType;
        public static UpdateTeamsMethodType StaticMethodType => UpdateTeamsMethodType.RemoveTeam;

        public void Write(PacketBuffer buffer) { }

        public static RemoveTeam Read(PacketBuffer buffer) => new();

        static IUpdateTeamsMethod IUpdateTeamsMethodStatic.Read(PacketBuffer buffer)
        {
            return Read(buffer);
        }
    }

    public sealed record UpdateTeamInfo(
        UpdateTeamsTeamInfoPart TeamInfoPart
    ) : IUpdateTeamsMethod, IUpdateTeamsMethodStatic, ISerializable<UpdateTeamInfo>
    {
        public UpdateTeamsMethodType MethodType => StaticMethodType;
        public static UpdateTeamsMethodType StaticMethodType => UpdateTeamsMethodType.UpdateTeamInfo;

        public void Write(PacketBuffer buffer)
        {
            TeamInfoPart.Write(buffer);
        }

        public static UpdateTeamInfo Read(PacketBuffer buffer)
        {
            var teamInfoPart = UpdateTeamsTeamInfoPart.Read(buffer);
            return new(teamInfoPart);
        }

        static IUpdateTeamsMethod IUpdateTeamsMethodStatic.Read(PacketBuffer buffer)
        {
            return Read(buffer);
        }
    }

    public sealed record AddEntitiesToTeam(
        UpdateTeamsEntityInfoPart EntityInfoPart
    ) : IUpdateTeamsMethod, IUpdateTeamsMethodStatic, ISerializable<AddEntitiesToTeam>
    {
        public UpdateTeamsMethodType MethodType => StaticMethodType;
        public static UpdateTeamsMethodType StaticMethodType => UpdateTeamsMethodType.AddEntitiesToTeam;

        public void Write(PacketBuffer buffer)
        {
            EntityInfoPart.Write(buffer);
        }

        public static AddEntitiesToTeam Read(PacketBuffer buffer)
        {
            var entityInfoPart = UpdateTeamsEntityInfoPart.Read(buffer);
            return new(entityInfoPart);
        }

        static IUpdateTeamsMethod IUpdateTeamsMethodStatic.Read(PacketBuffer buffer)
        {
            return Read(buffer);
        }
    }

    public sealed record RemoveEntitiesFromTeam(
        UpdateTeamsEntityInfoPart EntityInfoPart
    ) : IUpdateTeamsMethod, IUpdateTeamsMethodStatic, ISerializable<RemoveEntitiesFromTeam>
    {
        public UpdateTeamsMethodType MethodType => StaticMethodType;
        public static UpdateTeamsMethodType StaticMethodType => UpdateTeamsMethodType.RemoveEntitiesFromTeam;

        public void Write(PacketBuffer buffer)
        {
            EntityInfoPart.Write(buffer);
        }

        public static RemoveEntitiesFromTeam Read(PacketBuffer buffer)
        {
            var entityInfoPart = UpdateTeamsEntityInfoPart.Read(buffer);
            return new(entityInfoPart);
        }

        static IUpdateTeamsMethod IUpdateTeamsMethodStatic.Read(PacketBuffer buffer)
        {
            return Read(buffer);
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
