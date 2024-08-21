using System.Collections.Frozen;
using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.NetworkTypes;
using static MineSharp.Protocol.Packets.Clientbound.Play.PlayerInfoUpdatePacket;
using static MineSharp.Protocol.Packets.Clientbound.Play.PlayerInfoUpdatePacket.AddPlayerAction;
using static MineSharp.Protocol.Packets.Clientbound.Play.PlayerInfoUpdatePacket.InitializeChatAction;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

#pragma warning disable CS1591
public sealed partial record PlayerInfoUpdatePacket(int Action, ActionEntry[] Data) : IPacketStatic<PlayerInfoUpdatePacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_PlayerInfo;

    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        if (data.Version.Protocol >= ProtocolVersion.V_1_19_3)
        {
            buffer.WriteSByte((sbyte)Action);
        }
        else
        {
            buffer.WriteVarInt(Action);
        }

        buffer.WriteVarIntArray(Data, (buffer, actionData) => actionData.Write(buffer, data));
    }

    public static PlayerInfoUpdatePacket Read(PacketBuffer buffer, MinecraftData data)
    {
        int action;
        if (data.Version.Protocol >= ProtocolVersion.V_1_19_3)
        {
            action = buffer.ReadSByte();
        }
        else
        {
            action = buffer.ReadVarInt();
        }

        var actionData = buffer.ReadVarIntArray(buffer => ActionEntry.Read(buffer, data, action));

        return new(action, actionData);
    }

    public sealed record ActionEntry(Uuid Player, IPlayerInfoAction[] Actions) : ISerializableWithMinecraftData<ActionEntry>
    {
        public void Write(PacketBuffer buffer, MinecraftData data)
        {
            buffer.WriteUuid(Player);
            foreach (var action in Actions)
            {
                action.Write(buffer);
            }
        }

        public static ActionEntry Read(PacketBuffer buffer, MinecraftData data)
        {
            throw new NotImplementedException("This type does not support the normal Read method.");
        }

        public static ActionEntry Read(PacketBuffer buffer, MinecraftData data, int action)
        {
            var uuid = buffer.ReadUuid();
            var actions = new List<IPlayerInfoAction>();
            if (data.Version.Protocol <= ProtocolVersion.V_1_18_2)
            {
                switch (action)
                {
                    case 0:
                        actions.Add(AddPlayerAction.Read(buffer));
                        actions.Add(UpdateGameModeAction.Read(buffer));
                        actions.Add(UpdateLatencyAction.Read(buffer));
                        actions.Add(UpdateDisplayName.Read(buffer));
                        break;
                    case 1:
                        actions.Add(UpdateGameModeAction.Read(buffer));
                        break;
                    case 2:
                        actions.Add(UpdateLatencyAction.Read(buffer));
                        break;
                    case 3:
                        actions.Add(UpdateDisplayName.Read(buffer));
                        break;
                    case 4:
                        actions.Add(new UpdateListedAction(false));
                        break;
                }
            }
            else
            {
                foreach (var (mask, actionFactory) in ActionRegistry.ActionFactories)
                {
                    if ((action & mask) != 0)
                    {
                        actions.Add(actionFactory(buffer));
                    }
                }
            }

            return new(uuid, actions.ToArray());
        }
    }

    public static class ActionRegistry
    {
        public static readonly FrozenDictionary<int, Func<PacketBuffer, IPlayerInfoAction>> ActionFactories;

        static ActionRegistry()
        {
            ActionFactories = InitializeActions();
        }

        private static FrozenDictionary<int, Func<PacketBuffer, IPlayerInfoAction>> InitializeActions()
        {
            var dict = new Dictionary<int, Func<PacketBuffer, IPlayerInfoAction>>();

            void Register<T>()
                where T : IPlayerInfoAction, IPlayerInfoActionStatic
            {
                var mask = T.StaticMask;
                var factory = T.Read;
                dict.Add(mask, factory);
            }

            Register<AddPlayerAction>();
            Register<UpdateGameModeAction>();
            Register<UpdateListedAction>();
            Register<UpdateLatencyAction>();
            Register<UpdateDisplayName>();
            Register<InitializeChatAction>();

            return dict.ToFrozenDictionary();
        }
    }

    public interface IPlayerInfoAction
    {
        public int Mask { get; }

        public void Write(PacketBuffer buffer);
    }

    public interface IPlayerInfoActionStatic
    {
        public static abstract int StaticMask { get; }

        public static abstract IPlayerInfoAction Read(PacketBuffer buffer);
    }

    public sealed record AddPlayerAction(string Name, Property[] Properties) : IPlayerInfoAction, IPlayerInfoActionStatic, ISerializable<AddPlayerAction>
    {
        public static int StaticMask => 0x01;
        public int Mask => StaticMask;

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteString(Name);
            buffer.WriteVarIntArray(Properties, (packetBuffer, property) => property.Write(packetBuffer));
        }

        public static AddPlayerAction Read(PacketBuffer buffer)
        {
            var name = buffer.ReadString();
            var properties = buffer.ReadVarIntArray(Property.Read);
            return new(name, properties);
        }

        static IPlayerInfoAction IPlayerInfoActionStatic.Read(PacketBuffer buffer)
        {
            return Read(buffer);
        }

        public sealed record Property(string Name, string Value, string? Signature) : ISerializable<Property>
        {
            public void Write(PacketBuffer buffer)
            {
                buffer.WriteString(Name);
                buffer.WriteString(Value);

                var hasSignature = Signature != null;
                buffer.WriteBool(hasSignature);
                if (hasSignature)
                {
                    buffer.WriteString(Signature!);
                }
            }

            public static Property Read(PacketBuffer buffer)
            {
                var name = buffer.ReadString();
                var value = buffer.ReadString();
                var hasSignature = buffer.ReadBool();
                string? signature = null;
                if (hasSignature)
                {
                    signature = buffer.ReadString();
                }

                return new(name, value, signature);
            }
        }
    }

    public sealed record UpdateGameModeAction(GameMode GameMode) : IPlayerInfoAction, IPlayerInfoActionStatic, ISerializable<UpdateGameModeAction>
    {
        public static int StaticMask => 0x04;
        public int Mask => StaticMask;

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteVarInt((int)GameMode);
        }

        public static UpdateGameModeAction Read(PacketBuffer buffer)
        {
            var gameMode = (GameMode)buffer.ReadVarInt();
            return new(gameMode);
        }

        static IPlayerInfoAction IPlayerInfoActionStatic.Read(PacketBuffer buffer)
        {
            return Read(buffer);
        }
    }

    public sealed record UpdateListedAction(bool Listed) : IPlayerInfoAction, IPlayerInfoActionStatic, ISerializable<UpdateListedAction>
    {
        public static int StaticMask => 0x08;
        public int Mask => StaticMask;

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteBool(Listed);
        }

        public static UpdateListedAction Read(PacketBuffer buffer)
        {
            var listed = buffer.ReadBool();
            return new(listed);
        }

        static IPlayerInfoAction IPlayerInfoActionStatic.Read(PacketBuffer buffer)
        {
            return Read(buffer);
        }
    }

    public sealed record UpdateLatencyAction(int Ping) : IPlayerInfoAction, IPlayerInfoActionStatic, ISerializable<UpdateLatencyAction>
    {
        public static int StaticMask => 0x10;
        public int Mask => StaticMask;

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteVarInt(Ping);
        }

        public static UpdateLatencyAction Read(PacketBuffer buffer)
        {
            var ping = buffer.ReadVarInt();
            return new(ping);
        }

        static IPlayerInfoAction IPlayerInfoActionStatic.Read(PacketBuffer buffer)
        {
            return Read(buffer);
        }
    }

    public sealed record UpdateDisplayName(string? DisplayName) : IPlayerInfoAction, IPlayerInfoActionStatic, ISerializable<UpdateDisplayName>
    {
        public static int StaticMask => 0x20;
        public int Mask => StaticMask;

        public void Write(PacketBuffer buffer)
        {
            var hasDisplayName = DisplayName != null;
            buffer.WriteBool(hasDisplayName);
            if (hasDisplayName)
            {
                buffer.WriteString(DisplayName!);
            }
        }

        public static UpdateDisplayName Read(PacketBuffer buffer)
        {
            var hasDisplayName = buffer.ReadBool();
            string? displayName = null;
            if (hasDisplayName)
            {
                displayName = buffer.ReadString();
            }

            return new(displayName);
        }

        static IPlayerInfoAction IPlayerInfoActionStatic.Read(PacketBuffer buffer)
        {
            return Read(buffer);
        }
    }

    public sealed record InitializeChatAction(InitializeChatActionData? Data) : IPlayerInfoAction, IPlayerInfoActionStatic, ISerializable<InitializeChatAction>
    {
        public static int StaticMask => 0x02;
        public int Mask => StaticMask;

        public sealed record InitializeChatActionData(
            Uuid ChatSessionId,
            long PublicKeyExpiryTime,
            byte[] EncodedPublicKey,
            byte[] PublicKeySignature
        ) : ISerializable<InitializeChatActionData>
        {
            public void Write(PacketBuffer buffer)
            {
                buffer.WriteUuid(ChatSessionId);
                buffer.WriteLong(PublicKeyExpiryTime);
                buffer.WriteVarInt(EncodedPublicKey.Length);
                buffer.WriteBytes(EncodedPublicKey);
                buffer.WriteVarInt(PublicKeySignature.Length);
                buffer.WriteBytes(PublicKeySignature);
            }

            public static InitializeChatActionData Read(PacketBuffer buffer)
            {
                var chatSessionId = buffer.ReadUuid();
                var publicKeyExpiryTime = buffer.ReadLong();
                var encodedPublicKey = new byte[buffer.ReadVarInt()];
                buffer.ReadBytes(encodedPublicKey);
                var publicKeySignature = new byte[buffer.ReadVarInt()];
                buffer.ReadBytes(publicKeySignature);

                return new(chatSessionId, publicKeyExpiryTime, encodedPublicKey, publicKeySignature);
            }
        }

        public void Write(PacketBuffer buffer)
        {
            var present = Data != null;
            buffer.WriteBool(present);

            if (present)
            {
                Data!.Write(buffer);
            }
        }

        public static InitializeChatAction Read(PacketBuffer buffer)
        {
            var present = buffer.ReadBool();
            if (!present)
            {
                return new InitializeChatAction((InitializeChatActionData?)null);
            }

            var data = InitializeChatActionData.Read(buffer);

            return new(data);
        }

        static IPlayerInfoAction IPlayerInfoActionStatic.Read(PacketBuffer buffer)
        {
            return Read(buffer);
        }
    }
}
#pragma warning restore CS1591
