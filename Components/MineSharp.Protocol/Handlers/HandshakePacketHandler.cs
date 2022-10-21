using MineSharp.Core.Types.Enums;
using MineSharp.Data.Protocol;
using MineSharp.Data.Protocol.Handshaking.Serverbound;

namespace MineSharp.Protocol.Handlers
{
    internal class HandshakePacketHandler : IPacketHandler
    {
        public Task HandleIncomming(IPacketPayload packet, MinecraftClient client)
        {

            return packet switch {
                _ => Task.CompletedTask
            };

        }

        public Task HandleOutgoing(IPacketPayload packet, MinecraftClient client)
        {
            return packet switch {
                PacketSetProtocol setProtocol => this.HandleSetProtocolPacket(setProtocol, client),
                _ => Task.CompletedTask
            };
        }

        private Task HandleSetProtocolPacket(PacketSetProtocol packet, MinecraftClient client)
        {
            client.SetGameState((GameState)packet.NextState!.Value);
            return Task.CompletedTask;
        }
    }
}
