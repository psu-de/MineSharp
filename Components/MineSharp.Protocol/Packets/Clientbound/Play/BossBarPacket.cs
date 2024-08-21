using System.Collections.Frozen;
using MineSharp.ChatComponent;
using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using static MineSharp.Protocol.Packets.Clientbound.Play.BossBarPacket;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Boss bar packet
/// </summary>
/// <param name="Uuid">Unique ID for this bar</param>
/// <param name="Action">Determines the layout of the remaining packet</param>
public sealed partial record BossBarPacket(Uuid Uuid, IBossBarAction Action) : IPacketStatic<BossBarPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_BossBar;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteUuid(Uuid);
        buffer.WriteVarInt((int)Action.Type);
        Action.Write(buffer);
    }

    /// <inheritdoc />
    public static BossBarPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var uuid = buffer.ReadUuid();
        var actionType = (BossBarActionType)buffer.ReadVarInt();
        var action = BossBarActionRegistry.Read(buffer, actionType);
        return new(uuid, action);
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public interface IBossBarAction
    {
        public BossBarActionType Type { get; }
        public void Write(PacketBuffer buffer);
    }

    public interface IBossBarActionStatic
    {
        public static abstract BossBarActionType StaticType { get; }
        public static abstract IBossBarAction Read(PacketBuffer buffer);
    }

    public static class BossBarActionRegistry
    {
        public static IBossBarAction Read(PacketBuffer buffer, BossBarActionType type)
        {
            if (BossActionReaders.TryGetValue(type, out var reader))
            {
                return reader(buffer);
            }
            throw new ArgumentOutOfRangeException();
        }

        public static readonly FrozenDictionary<BossBarActionType, Func<PacketBuffer, IBossBarAction>> BossActionReaders;

        static BossBarActionRegistry()
        {
            BossActionReaders = InitializeBossActionReaders();
        }

        private static FrozenDictionary<BossBarActionType, Func<PacketBuffer, IBossBarAction>> InitializeBossActionReaders()
        {
            Dictionary<BossBarActionType, Func<PacketBuffer, IBossBarAction>> lookup = new();

            void Register<T>()
                where T : IBossBarAction, IBossBarActionStatic
            {
                var factory = T.Read;
                lookup.Add(T.StaticType, factory);
            }

            Register<AddBossBarAction>();
            Register<RemoveBossBarAction>();
            Register<UpdateHealthBossBarAction>();
            Register<UpdateTitleBossBarAction>();
            Register<UpdateStyleBossBarAction>();
            Register<UpdateFlagsBossBarAction>();

            return lookup.ToFrozenDictionary();
        }
    }

    public enum BossBarActionType
    {
        Add = 0,
        Remove = 1,
        UpdateHealth = 2,
        UpdateTitle = 3,
        UpdateStyle = 4,
        UpdateFlags = 5
    }

    public enum BossBarColor
    {
        Pink = 0,
        Blue = 1,
        Red = 2,
        Green = 3,
        Yellow = 4,
        Purple = 5,
        White = 6
    }

    public enum BossBarDivision
    {
        NoDivision = 0,
        SixNotches = 1,
        TenNotches = 2,
        TwelveNotches = 3,
        TwentyNotches = 4
    }

    public sealed record AddBossBarAction(Chat Title, float Health, BossBarColor Color, BossBarDivision Division, byte Flags) : IBossBarAction, IBossBarActionStatic, ISerializable<AddBossBarAction>
    {
        public BossBarActionType Type => StaticType;
        public static BossBarActionType StaticType => BossBarActionType.Add;

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteChatComponent(Title);
            buffer.WriteFloat(Health);
            buffer.WriteVarInt((int)Color);
            buffer.WriteVarInt((int)Division);
            buffer.WriteByte(Flags);
        }

        public static AddBossBarAction Read(PacketBuffer buffer)
        {
            var title = buffer.ReadChatComponent();
            var health = buffer.ReadFloat();
            var color = (BossBarColor)buffer.ReadVarInt();
            var division = (BossBarDivision)buffer.ReadVarInt();
            var flags = buffer.ReadByte();
            return new(title, health, color, division, flags);
        }

        static IBossBarAction IBossBarActionStatic.Read(PacketBuffer buffer)
        {
            return Read(buffer);
        }
    }

    public sealed record RemoveBossBarAction() : IBossBarAction, IBossBarActionStatic, ISerializable<RemoveBossBarAction>
    {
        public BossBarActionType Type => StaticType;
        public static BossBarActionType StaticType => BossBarActionType.Remove;

        public void Write(PacketBuffer buffer) { }

        public static RemoveBossBarAction Read(PacketBuffer buffer)
        {
            return new();
        }

        static IBossBarAction IBossBarActionStatic.Read(PacketBuffer buffer)
        {
            return Read(buffer);
        }
    }

    public sealed record UpdateHealthBossBarAction(float Health) : IBossBarAction, IBossBarActionStatic, ISerializable<UpdateHealthBossBarAction>
    {
        public BossBarActionType Type => StaticType;
        public static BossBarActionType StaticType => BossBarActionType.UpdateHealth;

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteFloat(Health);
        }

        public static UpdateHealthBossBarAction Read(PacketBuffer buffer)
        {
            var health = buffer.ReadFloat();
            return new(health);
        }

        static IBossBarAction IBossBarActionStatic.Read(PacketBuffer buffer)
        {
            return Read(buffer);
        }
    }

    public sealed record UpdateTitleBossBarAction(Chat Title) : IBossBarAction, IBossBarActionStatic, ISerializable<UpdateTitleBossBarAction>
    {
        public BossBarActionType Type => StaticType;
        public static BossBarActionType StaticType => BossBarActionType.UpdateTitle;

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteChatComponent(Title);
        }

        public static UpdateTitleBossBarAction Read(PacketBuffer buffer)
        {
            var title = buffer.ReadChatComponent();
            return new(title);
        }

        static IBossBarAction IBossBarActionStatic.Read(PacketBuffer buffer)
        {
            return Read(buffer);
        }
    }

    public sealed record UpdateStyleBossBarAction(BossBarColor Color, BossBarDivision Division) : IBossBarAction, IBossBarActionStatic, ISerializable<UpdateStyleBossBarAction>
    {
        public BossBarActionType Type => StaticType;
        public static BossBarActionType StaticType => BossBarActionType.UpdateStyle;

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteVarInt((int)Color);
            buffer.WriteVarInt((int)Division);
        }

        public static UpdateStyleBossBarAction Read(PacketBuffer buffer)
        {
            var color = (BossBarColor)buffer.ReadVarInt();
            var division = (BossBarDivision)buffer.ReadVarInt();
            return new(color, division);
        }

        static IBossBarAction IBossBarActionStatic.Read(PacketBuffer buffer)
        {
            return Read(buffer);
        }
    }

    public sealed record UpdateFlagsBossBarAction(byte Flags) : IBossBarAction, IBossBarActionStatic, ISerializable<UpdateFlagsBossBarAction>
    {
        public BossBarActionType Type => StaticType;
        public static BossBarActionType StaticType => BossBarActionType.UpdateFlags;

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteByte(Flags);
        }

        public static UpdateFlagsBossBarAction Read(PacketBuffer buffer)
        {
            var flags = buffer.ReadByte();
            return new(flags);
        }

        static IBossBarAction IBossBarActionStatic.Read(PacketBuffer buffer)
        {
            return Read(buffer);
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
