using MineSharp.Core.Types;

namespace MineSharp.Protocol.Packets.Clientbound.Login {
    public class LoginSuccessPacket : Packet {

        public UUID UUID { get; private set; }
        public string Username { get; private set; }

        public LoginSuccessPacket() { }
        public LoginSuccessPacket(UUID uuid, string username) {
            this.UUID = uuid;
            this.Username = username;
        }

        public override async Task Handle(MinecraftClient client) {
            client.SetGameState(GameState.PLAY);
            await base.Handle(client);
        }

        public override void Read(PacketBuffer buffer) {
            this.UUID = buffer.ReadUUID();
            this.Username = buffer.ReadString();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteUUID(this.UUID);
            buffer.WriteString(this.Username);
        }
    }
}
