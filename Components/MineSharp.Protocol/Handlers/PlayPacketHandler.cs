using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Handlers
{
    public class PlayPacketHandler : IPacketHandler
    {
        public Task HandleIncomming(IPacketPayload packet, MinecraftClient client)
        {
            return packet switch {
                MineSharp.Data.Protocol.Play.Clientbound.PacketKeepAlive keepAlive => this.HandleKeepAlive(keepAlive, client),
                _ => Task.CompletedTask
            };
        }

        public Task HandleOutgoing(IPacketPayload packet, MinecraftClient client) => Task.CompletedTask;

        private Task HandleKeepAlive(MineSharp.Data.Protocol.Play.Clientbound.PacketKeepAlive keepAlive, MinecraftClient client)
        {
            _ = client.SendPacket(new MineSharp.Data.Protocol.Play.Serverbound.PacketKeepAlive(keepAlive.KeepAliveId));
            return Task.CompletedTask;
        }
    }
}
