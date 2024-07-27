using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

#pragma warning disable CS1591
public class PlayerInfoUpdatePacket : IPacket
{
    public PlayerInfoUpdatePacket(int action, ActionEntry[] data)
    {
        Action = action;
        Data = data;
    }

    public int Action { get; set; }

    public ActionEntry[] Data { get; set; }
    public PacketType Type => PacketType.CB_Play_PlayerInfo;

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        if (version.Version.Protocol >= ProtocolVersion.V_1_19_3)
        {
            buffer.WriteSByte((sbyte)Action);
        }
        else
        {
            buffer.WriteVarInt(Action);
        }

        buffer.WriteVarInt(Data.Length);
        foreach (var data in Data)
        {
            data.Write(buffer);
        }
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        int action;
        if (version.Version.Protocol >= ProtocolVersion.V_1_19_3)
        {
            action = buffer.ReadByte();
        }
        else
        {
            action = buffer.ReadVarInt();
        }

        var data = buffer.ReadVarIntArray(buffer => ActionEntry.Read(buffer, version, action));
        return new PlayerInfoUpdatePacket(action, data);
    }

    public class ActionEntry
    {
        public ActionEntry(Uuid player, IPlayerInfoAction[] actions)
        {
            Player = player;
            Actions = actions;
        }

        public Uuid Player { get; set; }
        public IPlayerInfoAction[] Actions { get; set; }

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteUuid(Player);
            foreach (var action in Actions)
            {
                action.Write(buffer);
            }
        }

        public static ActionEntry Read(PacketBuffer buffer, MinecraftData version, int action)
        {
            var uuid = buffer.ReadUuid();
            var actions = new List<IPlayerInfoAction>();
            if (version.Version.Protocol <= ProtocolVersion.V_18_2)
            {
                switch (action)
                {
                    case 0:
                        actions.Add(AddPlayerAction.Read(buffer));
                        actions.Add(UpdateGameModeAction.Read(buffer));
                        actions.Add(UpdateLatencyAction.Read(buffer));
                        actions.Add(UpdateDisplayName.Read(buffer));

                        if (ProtocolVersion.IsBetween(version.Version.Protocol, ProtocolVersion.V_1_19,
                                                      ProtocolVersion.V_1_19_2))
                        {
                            actions.Add(CryptoActionV19.Read(buffer));
                        }

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
                if ((action & 0x01) != 0)
                {
                    actions.Add(AddPlayerAction.Read(buffer));
                }

                if ((action & 0x02) != 0)
                {
                    actions.Add(InitializeChatAction.Read(buffer));
                }

                if ((action & 0x04) != 0)
                {
                    actions.Add(UpdateGameModeAction.Read(buffer));
                }

                if ((action & 0x08) != 0)
                {
                    actions.Add(UpdateListedAction.Read(buffer));
                }

                if ((action & 0x10) != 0)
                {
                    actions.Add(UpdateLatencyAction.Read(buffer));
                }

                if ((action & 0x20) != 0)
                {
                    actions.Add(UpdateDisplayName.Read(buffer));
                }
            }

            return new(uuid, actions.ToArray());
        }
    }


    public interface IPlayerInfoAction
    {
        public void Write(PacketBuffer buffer);
    }

    public class AddPlayerAction : IPlayerInfoAction
    {
        public AddPlayerAction(string name, Property[] properties)
        {
            Name = name;
            Properties = properties;
        }

        public string Name { get; set; }
        public Property[] Properties { get; set; }

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

        public class Property
        {
            public Property(string name, string value, string? signature)
            {
                Name = name;
                Value = value;
                Signature = signature;
            }

            public string Name { get; set; }
            public string Value { get; set; }
            public string? Signature { get; set; }

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

    public class UpdateGameModeAction : IPlayerInfoAction
    {
        public UpdateGameModeAction(GameMode gameMode)
        {
            GameMode = gameMode;
        }

        public GameMode GameMode { get; set; }

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteVarInt((int)GameMode);
        }

        public static UpdateGameModeAction Read(PacketBuffer buffer)
        {
            var gameMode = (GameMode)buffer.ReadVarInt();
            return new(gameMode);
        }
    }

    public class UpdateListedAction : IPlayerInfoAction
    {
        public UpdateListedAction(bool listed)
        {
            Listed = listed;
        }

        public bool Listed { get; set; }

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteBool(Listed);
        }

        public static UpdateListedAction Read(PacketBuffer buffer)
        {
            var listed = buffer.ReadBool();
            return new(listed);
        }
    }

    public class UpdateLatencyAction : IPlayerInfoAction
    {
        public UpdateLatencyAction(int ping)
        {
            Ping = ping;
        }

        public int Ping { get; set; }

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteVarInt(Ping);
        }

        public static UpdateLatencyAction Read(PacketBuffer buffer)
        {
            var ping = buffer.ReadVarInt();
            return new(ping);
        }
    }

    public class UpdateDisplayName : IPlayerInfoAction
    {
        public UpdateDisplayName(string? displayName)
        {
            DisplayName = displayName;
        }

        public string? DisplayName { get; set; }

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
    }

    public class InitializeChatAction : IPlayerInfoAction
    {
        public InitializeChatAction()
        {
            Present = false;
        }

        public InitializeChatAction(Uuid chatSessionId, long publicKeyExpiryTime, byte[] encodedPublicKey,
                                    byte[] publicKeySignature)
        {
            Present = true;
            ChatSessionId = chatSessionId;
            PublicKeyExpiryTime = publicKeyExpiryTime;
            EncodedPublicKey = encodedPublicKey;
            PublicKeySignature = publicKeySignature;
        }

        public bool Present { get; set; }
        public Uuid? ChatSessionId { get; set; }
        public long? PublicKeyExpiryTime { get; set; }
        public byte[]? EncodedPublicKey { get; set; }
        public byte[]? PublicKeySignature { get; set; }

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteBool(Present);

            if (!Present)
            {
                return;
            }

            buffer.WriteUuid(ChatSessionId!.Value);
            buffer.WriteLong(PublicKeyExpiryTime!.Value);
            buffer.WriteVarInt(EncodedPublicKey!.Length);
            buffer.WriteBytes(EncodedPublicKey);
            buffer.WriteVarInt(PublicKeySignature!.Length);
            buffer.WriteBytes(PublicKeySignature);
        }

        public static InitializeChatAction Read(PacketBuffer buffer)
        {
            var present = buffer.ReadBool();
            if (!present)
            {
                return new();
            }

            var chatSessionId = buffer.ReadUuid();
            var publicKeyExpiryTime = buffer.ReadLong();
            var encodedPublicKey = new byte[buffer.ReadVarInt()];
            buffer.ReadBytes(encodedPublicKey);
            var publicKeySignature = new byte[buffer.ReadVarInt()];
            buffer.ReadBytes(publicKeySignature);

            return new(chatSessionId, publicKeyExpiryTime, encodedPublicKey, publicKeySignature);
        }
    }

    public class CryptoActionV19 : IPlayerInfoAction
    {
        public CryptoActionV19()
        {
            Present = false;
        }

        public CryptoActionV19(Uuid chatSessionId, long timestamp, byte[] encodedPublicKey, byte[] publicKeySignature)
        {
            Present = true;
            Timestamp = timestamp;
            EncodedPublicKey = encodedPublicKey;
            PublicKeySignature = publicKeySignature;
        }

        public bool Present { get; set; }
        public long? Timestamp { get; set; }
        public byte[]? EncodedPublicKey { get; set; }
        public byte[]? PublicKeySignature { get; set; }

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteBool(Present);

            if (!Present)
            {
                return;
            }

            buffer.WriteLong(Timestamp!.Value);
            buffer.WriteVarInt(EncodedPublicKey!.Length);
            buffer.WriteBytes(EncodedPublicKey);
            buffer.WriteVarInt(PublicKeySignature!.Length);
            buffer.WriteBytes(PublicKeySignature);
        }

        public static InitializeChatAction Read(PacketBuffer buffer)
        {
            var present = buffer.ReadBool();
            if (!present)
            {
                return new();
            }

            var chatSessionId = buffer.ReadUuid();
            var publicKeyExpiryTime = buffer.ReadLong();
            var encodedPublicKey = new byte[buffer.ReadVarInt()];
            buffer.ReadBytes(encodedPublicKey);
            var publicKeySignature = new byte[buffer.ReadVarInt()];
            buffer.ReadBytes(publicKeySignature);

            return new(chatSessionId, publicKeyExpiryTime, encodedPublicKey, publicKeySignature);
        }
    }
}
#pragma warning restore CS1591
