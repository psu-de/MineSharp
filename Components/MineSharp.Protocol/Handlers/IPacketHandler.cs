using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Handlers
{
    public interface IPacketHandler
    {
        public Task HandleIncomming(IPacketPayload packet, MinecraftClient client);
        public Task HandleOutgoing(IPacketPayload packet, MinecraftClient client);
    }
}
