using MineSharp.Core.Common.Protocol;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Handlers;

internal abstract class GameStatePacketHandler : IPacketHandler
{
    public readonly GameState GameState;

    protected GameStatePacketHandler(GameState gameState)
    {
        GameState = gameState;
    }

    public abstract Task HandleIncoming(IPacket packet);
    public abstract Task HandleOutgoing(IPacket packet);
    public abstract bool HandlesIncoming(PacketType type);
}
