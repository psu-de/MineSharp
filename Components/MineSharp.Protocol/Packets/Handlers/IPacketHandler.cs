namespace MineSharp.Protocol.Packets.Handlers;

public interface IPacketHandler
{
    public Task HandleIncoming(IPacket packet);
    public Task HandleOutgoing(IPacket packet);
}
