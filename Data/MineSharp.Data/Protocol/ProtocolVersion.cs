namespace MineSharp.Data.Protocol;

internal abstract class ProtocolVersion
{
    public abstract Dictionary<PacketType, int> PacketIds { get; }
}
