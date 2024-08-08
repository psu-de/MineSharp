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
public sealed record BossBarPacket(Uuid Uuid, BossBarAction Action) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_BossBar;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteUuid(Uuid);
        buffer.WriteVarInt((int)Action.Type);
        Action.Write(buffer);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var uuid = buffer.ReadUuid();
        var actionType = (BossBarActionType)buffer.ReadVarInt();
        var action = BossBarAction.Read(buffer, actionType);
        return new BossBarPacket(uuid, action);
    }


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    /// <summary>
    ///     Represents the action of a boss bar packet
    /// </summary>
    public interface IBossBarAction<T> where T : BossBarAction
    {
        static abstract BossBarActionType StaticType { get; }
        void Write(PacketBuffer buffer);
        static abstract T Read(PacketBuffer buffer);
    }

    public abstract record BossBarAction
    {
        public abstract BossBarActionType Type { get; }
        public abstract void Write(PacketBuffer buffer);

        public static BossBarAction Read(PacketBuffer buffer, BossBarActionType type)
        {
            if (BossActionReaders.TryGetValue(type, out var reader))
            {
                return reader(buffer);
            }
            throw new ArgumentOutOfRangeException();
        }

        public static readonly FrozenDictionary<BossBarActionType, Func<PacketBuffer, BossBarAction>> BossActionReaders = CreateBossActionReadersLookup();

        private static FrozenDictionary<BossBarActionType, Func<PacketBuffer, BossBarAction>> CreateBossActionReadersLookup()
        {
            Dictionary<BossBarActionType, Func<PacketBuffer, BossBarAction>> lookup = new();

            void Register<T>() where T : BossBarAction, IBossBarAction<T>
            {
                lookup.Add(T.StaticType, buffer => T.Read(buffer));
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

    public sealed record AddBossBarAction(Chat Title, float Health, BossBarColor Color, BossBarDivision Division, byte Flags) : BossBarAction, IBossBarAction<AddBossBarAction>
    {
        public override BossBarActionType Type => StaticType;
        public static BossBarActionType StaticType => BossBarActionType.Add;

        public override void Write(PacketBuffer buffer)
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
            return new AddBossBarAction(title, health, color, division, flags);
        }
    }

    public sealed record RemoveBossBarAction() : BossBarAction(), IBossBarAction<RemoveBossBarAction>
    {
        public override BossBarActionType Type => StaticType;
        public static BossBarActionType StaticType => BossBarActionType.Remove;

        public override void Write(PacketBuffer buffer) { }

        public static RemoveBossBarAction Read(PacketBuffer buffer)
        {
            return new RemoveBossBarAction();
        }
    }

    public sealed record UpdateHealthBossBarAction(float Health) : BossBarAction, IBossBarAction<UpdateHealthBossBarAction>
    {
        public override BossBarActionType Type => StaticType;
        public static BossBarActionType StaticType => BossBarActionType.UpdateHealth;

        public override void Write(PacketBuffer buffer)
        {
            buffer.WriteFloat(Health);
        }

        public static UpdateHealthBossBarAction Read(PacketBuffer buffer)
        {
            var health = buffer.ReadFloat();
            return new UpdateHealthBossBarAction(health);
        }
    }

    public sealed record UpdateTitleBossBarAction(Chat Title) : BossBarAction, IBossBarAction<UpdateTitleBossBarAction>
    {
        public override BossBarActionType Type => StaticType;
        public static BossBarActionType StaticType => BossBarActionType.UpdateTitle;

        public override void Write(PacketBuffer buffer)
        {
            buffer.WriteChatComponent(Title);
        }

        public static UpdateTitleBossBarAction Read(PacketBuffer buffer)
        {
            var title = buffer.ReadChatComponent();
            return new UpdateTitleBossBarAction(title);
        }
    }

    public sealed record UpdateStyleBossBarAction(BossBarColor Color, BossBarDivision Division) : BossBarAction, IBossBarAction<UpdateStyleBossBarAction>
    {
        public override BossBarActionType Type => StaticType;
        public static BossBarActionType StaticType => BossBarActionType.UpdateStyle;

        public override void Write(PacketBuffer buffer)
        {
            buffer.WriteVarInt((int)Color);
            buffer.WriteVarInt((int)Division);
        }

        public static UpdateStyleBossBarAction Read(PacketBuffer buffer)
        {
            var color = (BossBarColor)buffer.ReadVarInt();
            var division = (BossBarDivision)buffer.ReadVarInt();
            return new UpdateStyleBossBarAction(color, division);
        }
    }

    public sealed record UpdateFlagsBossBarAction(byte Flags) : BossBarAction, IBossBarAction<UpdateFlagsBossBarAction>
    {
        public override BossBarActionType Type => StaticType;
        public static BossBarActionType StaticType => BossBarActionType.UpdateFlags;

        public override void Write(PacketBuffer buffer)
        {
            buffer.WriteByte(Flags);
        }

        public static UpdateFlagsBossBarAction Read(PacketBuffer buffer)
        {
            var flags = buffer.ReadByte();
            return new UpdateFlagsBossBarAction(flags);
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
