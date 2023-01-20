using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Handlers
{
    public class StatusPacketHandler : IPacketHandler
    {

        public Task HandleIncoming(IPacketPayload packet, MinecraftClient client) => throw new NotImplementedException();
        public Task HandleOutgoing(IPacketPayload packet, MinecraftClient client) => throw new NotImplementedException();
    }
}
