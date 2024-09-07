namespace MineSharp.Protocol.Registrations;

/// <summary>
/// Abstract class a disposable registration.
/// </summary>
public abstract class AbstractDisposableRegistration : IDisposable
{
    private int disposedValue;
    /// <summary>
    /// Whether this registration is disposed and the handler is unregistered.
    /// </summary>
    public bool Disposed => disposedValue != 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractDisposableRegistration"/> class.
    /// </summary>
    protected AbstractDisposableRegistration()
    {
        disposedValue = 0;
    }

    /// <summary>
    /// This method unregisters the the registered object.
    /// Must be overridden by subclasses.
    /// 
    /// This method is called when the registration is disposed.
    /// </summary>
    protected abstract void Unregister();

    ///<inheritdoc/>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref disposedValue, 1) != 0)
        {
            // Already disposed
            return;
        }
        Unregister();
    }
}
