using MineSharp.Data.Protocol;
using MineSharp.Data.Protocol.Play.Clientbound;

namespace MineSharp.Protocol.Handlers
{
    public class PlayPacketHandler : IPacketHandler
    {
        public Task HandleIncomming(IPacketPayload packet, MinecraftClient client)
        {
            return packet switch {
                PacketKeepAlive keepAlive => this.HandleKeepAlive(keepAlive, client),
                _ => Task.CompletedTask
            };
        }

        public Task HandleOutgoing(IPacketPayload packet, MinecraftClient client) => Task.CompletedTask;

        private Task HandleKeepAlive(PacketKeepAlive keepAlive, MinecraftClient client)
        {
            _ = client.SendPacket(new Data.Protocol.Play.Serverbound.PacketKeepAlive(keepAlive.KeepAliveId));
            return Task.CompletedTask;
        }
    }
}
