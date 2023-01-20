using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Handlers
{
    internal interface IPacketHandler
    {
        public Task HandleIncoming(IPacketPayload packet, MinecraftClient client);
        public Task HandleOutgoing(IPacketPayload packet, MinecraftClient client);
    }
}
