using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;

namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class PlayerInfoPacket : Packet {
        public PlayerInfoAction Action { get; private set; }
        public (UUID, object)[]? Players { get; private set; }


        public override void Read(PacketBuffer buffer) {
            this.Action = (PlayerInfoAction)buffer.ReadVarInt();

            int length = buffer.ReadVarInt();
            this.Players = new (UUID, object)[length];
            for (int i = 0; i < length; i++) {
                UUID uuid = buffer.ReadUUID();
                object? props = null;
                switch (this.Action) {
                    case PlayerInfoAction.AddPlayer:
                        string name = buffer.ReadString();
                        int count = buffer.ReadVarInt();
                        PlayerInfoProperty[] properties = new PlayerInfoProperty[count];
                        for (int j = 0; j < count; j++) {
                            string pName = buffer.ReadString();
                            string pValue = buffer.ReadString();
                            bool pSigned = buffer.ReadBoolean();
                            string? pSignature = null;
                            if (pSigned) pSignature = buffer.ReadString();
                            properties[j] = new PlayerInfoProperty() { Name = pName, Value = pValue, IsSigned = pSigned, Signature = pSignature };
                        }
                        GameMode gameMode = (GameMode)buffer.ReadVarInt();
                        int ping = buffer.ReadVarInt();
                        bool hasDisplayName = buffer.ReadBoolean();
                        Chat? displayName = null;
                        if (hasDisplayName) displayName = buffer.ReadChat();
                        props = new PlayerInfo() { Name = name, Properties = properties, GameMode = gameMode, Ping = ping, HasDisplayName = hasDisplayName, DisplayName = displayName };
                        break;
                    case PlayerInfoAction.UpdateGamemode:
                        props = new { Gamemode = (GameMode)buffer.ReadVarInt() };
                        break;
                    case PlayerInfoAction.UpdateLatency:
                        props = new { Ping = buffer.ReadVarInt() };
                        break;
                    case PlayerInfoAction.UpdateDisplayName:
                        bool hasdName = buffer.ReadBoolean();
                        Chat? dName = null;
                        if (hasdName) dName = buffer.ReadChat();
                        props = new { HasDisplayName = hasdName, DisplayName = dName };
                        break;
                    case PlayerInfoAction.RemovePlayer:
                        break;
                }

                this.Players[i] = (uuid!, props!);
            }
        }

        public override void Write(PacketBuffer buffer) {
            throw new NotImplementedException();
        }

        public enum PlayerInfoAction {
            AddPlayer = 0,
            UpdateGamemode = 1,
            UpdateLatency = 2,
            UpdateDisplayName = 3,
            RemovePlayer = 4
        }



        public struct PlayerInfo {
            public string Name;
            public PlayerInfoProperty[] Properties;
            public GameMode GameMode;
            public int Ping;
            public bool HasDisplayName;
            public Chat? DisplayName;
        }

        public struct PlayerInfoProperty {
            public string Name;
            public string Value;
            public bool IsSigned;
            public string? Signature;
        }
    }
}
