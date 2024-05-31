using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Handlers.Client;

internal interface IPacketHandler
{
    public Task HandleIncoming(IPacket packet);
    public Task HandleOutgoing(IPacket packet);

    public bool HandlesIncoming(PacketType type);
}
