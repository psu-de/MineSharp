using MineSharp.Core.Common.Protocol;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Handlers;

internal abstract class GameStatePacketHandler
{
    public readonly GameState GameState;

    protected GameStatePacketHandler(GameState gameState)
    {
        GameState = gameState;
    }

    public virtual Task StateEntered()
    {
        return Task.CompletedTask;
    }
    public abstract Task HandleIncoming(IPacket packet);
    // normally this is not required
    public virtual Task HandleOutgoing(IPacket packet)
    {
        return Task.CompletedTask;
    }
    public abstract bool HandlesIncoming(PacketType type);
}
