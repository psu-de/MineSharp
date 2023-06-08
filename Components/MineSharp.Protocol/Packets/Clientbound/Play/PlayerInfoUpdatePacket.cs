using MineSharp.Core.Common;
using MineSharp.Data;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

public class PlayerInfoUpdatePacket : IPacket
{
    public static int Id => 0x3A;
    
    public int Action { get; set; }
    
    public ActionEntry[] Data { get; set; }

    public PlayerInfoUpdatePacket(int action, ActionEntry[] data)
    {
        this.Action = action;
        this.Data = data;
    }

    public void Write(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        if (version.Protocol.Version >= ProtocolVersion.V_1_19_3)
            buffer.WriteSByte((sbyte)this.Action);
        else 
            buffer.WriteVarInt(this.Action);
        
        buffer.WriteVarInt(this.Data.Length);
        foreach (var data in this.Data)
        {
            data.Write(buffer);
        }
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        int action;
        if (version.Protocol.Version >= ProtocolVersion.V_1_19_3)
            action = buffer.ReadByte();
        else 
            action = buffer.ReadVarInt();

        var data = buffer.ReadVarIntArray(buffer => ActionEntry.Read(buffer, version, action));
        return new PlayerInfoUpdatePacket(action, data);
    }

    public class ActionEntry
    {
        public UUID Player { get; set; }
        public IPlayerInfoAction[] Actions { get; set; }

        public ActionEntry(UUID player, IPlayerInfoAction[] actions)
        {
            this.Player = player;
            this.Actions = actions;
        }

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteUuid(this.Player);
            foreach (var action in this.Actions)
                action.Write(buffer);
        }

        public static ActionEntry Read(PacketBuffer buffer, MinecraftData version, int action)
        {
            var uuid = buffer.ReadUuid();
            var actions = new List<IPlayerInfoAction>();
            if (version.Protocol.Version <= ProtocolVersion.V_18_2)
            {
                switch (action)
                {
                    case 0:
                        actions.Add(AddPlayerAction.Read(buffer));
                        actions.Add(UpdateGameModeAction.Read(buffer));
                        actions.Add(UpdateLatencyAction.Read(buffer));
                        actions.Add(UpdateDisplayName.Read(buffer));

                        if (ProtocolVersion.IsBetween(version.Protocol.Version, ProtocolVersion.V_1_19, ProtocolVersion.V_1_19_2))
                            actions.Add(CryptoActionV19.Read(buffer));
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
                    actions.Add(AddPlayerAction.Read(buffer));
                if ((action & 0x02) != 0) 
                    actions.Add(InitializeChatAction.Read(buffer));
                if ((action & 0x04) != 0)
                    actions.Add(UpdateGameModeAction.Read(buffer));
                if((action & 0x08) != 0)
                    actions.Add(UpdateListedAction.Read(buffer));
                if ((action & 0x10) != 0)
                    actions.Add(UpdateLatencyAction.Read(buffer));
                if ((action & 0x20) != 0) 
                    actions.Add(UpdateDisplayName.Read(buffer));
            }

            return new ActionEntry(uuid, actions.ToArray());
        }
    }


    public interface IPlayerInfoAction
    {
        public void Write(PacketBuffer buffer);
    }

    public class AddPlayerAction : IPlayerInfoAction
    {
        public string Name { get; set; }
        public Property[] Properties { get; set; }

        public AddPlayerAction(string name, Property[] properties)
        {
            this.Name = name;
            this.Properties = properties;
        }

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteString(this.Name);
            buffer.WriteVarIntArray(this.Properties, (packetBuffer, property) => property.Write(packetBuffer));
        }

        public static AddPlayerAction Read(PacketBuffer buffer)
        {
            var name = buffer.ReadString();
            var properties = buffer.ReadVarIntArray(Property.Read);
            return new AddPlayerAction(name, properties);
        }

        public class Property
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public string? Signature { get; set; }

            public Property(string name, string value, string? signature)
            {
                this.Name = name;
                this.Value = value;
                this.Signature = signature;
            }

            public void Write(PacketBuffer buffer)
            {
                buffer.WriteString(this.Name);
                buffer.WriteString(this.Value);

                var hasSignature = this.Signature != null;
                buffer.WriteBool(hasSignature);
                if (hasSignature)
                    buffer.WriteString(this.Signature!);
            }

            public static Property Read(PacketBuffer buffer)
            {
                var name = buffer.ReadString();
                var value = buffer.ReadString();
                var hasSignature = buffer.ReadBool();
                string? signature = null;
                if (hasSignature)
                    signature = buffer.ReadString();
                return new Property(name, value, signature);
            }
        }
    }

    public class UpdateGameModeAction : IPlayerInfoAction
    {
        public GameMode GameMode { get; set; }

        public UpdateGameModeAction(GameMode gameMode)
        {
            this.GameMode = gameMode;
        }

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteVarInt((int)this.GameMode);
        }

        public static UpdateGameModeAction Read(PacketBuffer buffer)
        {
            var gameMode = (GameMode)buffer.ReadVarInt();
            return new UpdateGameModeAction(gameMode);
        }
    }

    public class UpdateListedAction : IPlayerInfoAction
    {
        public bool Listed { get; set; }

        public UpdateListedAction(bool listed)
        {
            this.Listed = listed;
        }

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteBool(this.Listed);
        }

        public static UpdateListedAction Read(PacketBuffer buffer)
        {
            var listed = buffer.ReadBool();
            return new UpdateListedAction(listed);
        }
    }

    public class UpdateLatencyAction : IPlayerInfoAction
    {
        public int Ping { get; set; }

        public UpdateLatencyAction(int ping)
        {
            this.Ping = ping;
        }

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteVarInt(this.Ping);
        }

        public static UpdateLatencyAction Read(PacketBuffer buffer)
        {
            var ping = buffer.ReadVarInt();
            return new UpdateLatencyAction(ping);
        }
    }

    public class UpdateDisplayName : IPlayerInfoAction
    {
        public string? DisplayName { get; set; }

        public UpdateDisplayName(string? displayName)
        {
            this.DisplayName = displayName;
        }

        public void Write(PacketBuffer buffer)
        {
            var hasDisplayName = this.DisplayName != null;
            buffer.WriteBool(hasDisplayName);
            if (hasDisplayName)
                buffer.WriteString(this.DisplayName!);
        }

        public static UpdateDisplayName Read(PacketBuffer buffer)
        {
            var hasDisplayName = buffer.ReadBool();
            string? displayName = null;
            if (hasDisplayName)
                displayName = buffer.ReadString();
            return new UpdateDisplayName(displayName);
        }
    }

    public class InitializeChatAction : IPlayerInfoAction
    {
        public bool Present { get; set; }
        public UUID? ChatSessionId { get; set; }
        public long? PublicKeyExpiryTime { get; set; }
        public byte[]? EncodedPublicKey { get; set; }
        public byte[]? PublicKeySignature { get; set; }

        public InitializeChatAction()
        {
            this.Present = false;
        }
        
        public InitializeChatAction(UUID chatSessionId, long publicKeyExpiryTime, byte[] encodedPublicKey, byte[] publicKeySignature)
        {
            this.Present = true;
            this.ChatSessionId = chatSessionId;
            this.PublicKeyExpiryTime = publicKeyExpiryTime;
            this.EncodedPublicKey = encodedPublicKey;
            this.PublicKeySignature = publicKeySignature;
        }

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteBool(this.Present);

            if (!this.Present)
                return;
            
            buffer.WriteUuid(this.ChatSessionId!.Value);
            buffer.WriteLong(this.PublicKeyExpiryTime!.Value);
            buffer.WriteVarInt(this.EncodedPublicKey!.Length);
            buffer.WriteBytes(this.EncodedPublicKey);
            buffer.WriteVarInt(this.PublicKeySignature!.Length);
            buffer.WriteBytes(this.PublicKeySignature);
        }

        public static InitializeChatAction Read(PacketBuffer buffer)
        {
            var present = buffer.ReadBool();
            if (!present)
                return new InitializeChatAction();

            var chatSessionId = buffer.ReadUuid();
            var publicKeyExpiryTime = buffer.ReadLong();
            var encodedPublicKey = new byte[buffer.ReadVarInt()];
            buffer.ReadBytes(encodedPublicKey);
            var publicKeySignature = new byte[buffer.ReadVarInt()];
            buffer.ReadBytes(publicKeySignature);
            
            return new InitializeChatAction(chatSessionId, publicKeyExpiryTime, encodedPublicKey, publicKeySignature);
        }
    }

    public class CryptoActionV19 : IPlayerInfoAction
    {
        public bool Present { get; set; }
        public long? Timestamp { get; set; }
        public byte[]? EncodedPublicKey { get; set; }
        public byte[]? PublicKeySignature { get; set; }

        public CryptoActionV19()
        {
            this.Present = false;
        }
        
        public CryptoActionV19(UUID chatSessionId, long timestamp, byte[] encodedPublicKey, byte[] publicKeySignature)
        {
            this.Present = true;
            this.Timestamp = timestamp;
            this.EncodedPublicKey = encodedPublicKey;
            this.PublicKeySignature = publicKeySignature;
        }

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteBool(this.Present);

            if (!this.Present)
                return;
            
            buffer.WriteLong(this.Timestamp!.Value);
            buffer.WriteVarInt(this.EncodedPublicKey!.Length);
            buffer.WriteBytes(this.EncodedPublicKey);
            buffer.WriteVarInt(this.PublicKeySignature!.Length);
            buffer.WriteBytes(this.PublicKeySignature);
        }

        public static InitializeChatAction Read(PacketBuffer buffer)
        {
            var present = buffer.ReadBool();
            if (!present)
                return new InitializeChatAction();

            var chatSessionId = buffer.ReadUuid();
            var publicKeyExpiryTime = buffer.ReadLong();
            var encodedPublicKey = new byte[buffer.ReadVarInt()];
            buffer.ReadBytes(encodedPublicKey);
            var publicKeySignature = new byte[buffer.ReadVarInt()];
            buffer.ReadBytes(publicKeySignature);
            
            return new InitializeChatAction(chatSessionId, publicKeyExpiryTime, encodedPublicKey, publicKeySignature);
        }
    }
}
