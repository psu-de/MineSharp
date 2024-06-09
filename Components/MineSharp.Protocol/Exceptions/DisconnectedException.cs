namespace MineSharp.Protocol.Exceptions;

/// <inheritdoc />
public class DisconnectedException(string message, string reason) : MineSharpHostException(message)
{
    /// <summary>
    /// The reason of the disconnect
    /// </summary>
    public string Reason { get; private set; } = reason;
}
