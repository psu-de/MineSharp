using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;

namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class BossBarPacket : Packet {

        public UUID UUID { get; private set; }
        public BossBarAction Action { get; private set; }
        public Chat? Title { get; private set; }
        public float? Health { get; private set; }
        public MinecraftColors? Color { get; private set; }
        public Division? Dividers { get; private set; }
        public byte? Flags { get; private set; }

        public BossBarPacket() { }

        public BossBarPacket(UUID uUID, BossBarAction action, Chat? title, float? health, MinecraftColors? color, Division? division, byte? flags) {
            UUID = uUID;
            Action = action;
            Title = title;
            Health = health;
            Color = color;
            Dividers = division;
            Flags = flags;
        }

        public override void Read(PacketBuffer buffer) {
            this.UUID = buffer.ReadUUID();
            this.Action = (BossBarAction)buffer.ReadVarInt();

            switch (this.Action) {
                case BossBarAction.Add:
                    this.Title = buffer.ReadChat();
                    this.Health = buffer.ReadFloat();
                    this.Color = (MinecraftColors)buffer.ReadVarInt();
                    this.Dividers = (Division)buffer.ReadVarInt();
                    this.Flags = buffer.ReadByte();
                    break;
                case BossBarAction.Remove:
                    break;
                case BossBarAction.UpdateHealth:
                    this.Health = buffer.ReadVarInt();
                    break;
                case BossBarAction.UpdateTitle:
                    this.Title = buffer.ReadChat();
                    break;
                case BossBarAction.UpdateStyle:
                    this.Color = (MinecraftColors)buffer.ReadVarInt();
                    this.Dividers = (Division)buffer.ReadVarInt();
                    break;
                case BossBarAction.UpdateFlags:
                    this.Flags = buffer.ReadByte();
                    break;
            }
        }

        public override void Write(PacketBuffer buffer) {
            throw new NotImplementedException();
        }
    }
}
