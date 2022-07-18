using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Handlers {
    internal class HandshakePacketHandler : IPacketHandler {
        public Task HandleIncomming(IPacketPayload packet, MinecraftClient client) {

            return packet switch {
                _ => Task.CompletedTask
            };

        }

        public Task HandleOutgoing(IPacketPayload packet, MinecraftClient client) {
            return packet switch {
                MineSharp.Data.Protocol.Handshaking.Serverbound.PacketSetProtocol setProtocol => HandleSetProtocolPacket(setProtocol, client),
                _ => Task.CompletedTask,
            };
        }

        private Task HandleSetProtocolPacket(MineSharp.Data.Protocol.Handshaking.Serverbound.PacketSetProtocol packet, MinecraftClient client) {
            client.SetGameState((Core.Types.Enums.GameState)packet.NextState!.Value);
            return Task.CompletedTask;
        }
    }
}
