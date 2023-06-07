using MineSharp.Core.Exceptions;

namespace MineSharp.Protocol.Exceptions;

public class PacketVersionException : MineSharpException
{

    public PacketVersionException(string message) : base(message)
    { }
}
