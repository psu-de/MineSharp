using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Handlers {
    internal interface IPacketHandler {
        public Task HandleIncomming(IPacketPayload packet, MinecraftClient client);
        public Task HandleOutgoing(IPacketPayload packet, MinecraftClient client);
    }
}
