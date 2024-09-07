using ConcurrentCollections;
using MineSharp.Protocol.Packets;
using MineSharp.Protocol.Registrations;
using NLog;
using static MineSharp.Protocol.MinecraftClient;

namespace MineSharp.Bot.Plugins;

/// <summary>
///     Plugin for <see cref="MineSharpBot" />.
/// </summary>
public abstract class Plugin : IAsyncDisposable
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private readonly TaskCompletionSource initializationTask;

    /// <summary>
    ///     The bot
    /// </summary>
    protected readonly MineSharpBot Bot;

    /// <summary>
    ///    All the registrations for packet handlers that this plugin has mode.
    ///    Used to unregister them when the plugin is disposed.
    /// </summary>
    protected readonly ConcurrentHashSet<AbstractPacketReceiveRegistration> PacketReceiveRegistrations;

    /// <summary>
    ///     Create a new Plugin instance
    /// </summary>
    /// <param name="bot"></param>
    protected Plugin(MineSharpBot bot)
    {
        Bot = bot;
        IsEnabled = true;
        initializationTask = new(TaskCreationOptions.RunContinuationsAsynchronously);
        PacketReceiveRegistrations = new();
    }

    /// <summary>
    ///     Whether this plugin is currently enabled
    /// </summary>
    public bool IsEnabled { get; private set; }

    /// <summary>
    ///     Whether this plugin is loaded and functional
    ///     
    /// <seealso cref="WaitForInitialization"/>
    /// </summary>
    public bool IsLoaded => initializationTask.Task.IsCompleted;

    /// <summary>
    ///     This method is called once when the plugin starts.
    ///     It should stop (cancel) when the <see cref="MineSharpBot.CancellationToken"/> is cancelled.
    /// </summary>
    /// <returns></returns>
    protected virtual Task Init()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    ///     This method is called each minecraft tick. (About 20 times a second.)
    ///     It should stop (cancel) when the <see cref="MineSharpBot.CancellationToken"/> is cancelled.
    /// </summary>
    /// <returns></returns>
    protected internal virtual Task OnTick()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    ///     This method is called when the plugin gets enabled.
    /// </summary>
    /// <returns></returns>
    protected virtual Task OnEnable()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    ///     This method is called when the plugin gets disabled.
    /// </summary>
    /// <returns></returns>
    protected virtual Task OnDisable()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Enable or disable the plugin.
    /// </summary>
    /// <param name="enabled"></param>
    public async Task SetEnabled(bool enabled)
    {
        if (IsEnabled == enabled)
        {
            return;
        }

        if (enabled)
        {
            await OnEnable();
        }
        else
        {
            await OnDisable();
        }

        IsEnabled = enabled;
    }

    /// <summary>
    ///     Returns a task that resolves once this module has been initialized.
    /// </summary>
    /// <returns></returns>
    public Task WaitForInitialization()
    {
        return initializationTask.Task;
    }

    private AsyncPacketHandler<TPacket> CreateAfterInitializationPacketHandlerWrapper<TPacket>(AsyncPacketHandler<TPacket> packetHandler, bool queuePacketsSentBeforeInitializationCompleted = false)
        where TPacket : IPacket
    {
        return async param =>
        {
            if (queuePacketsSentBeforeInitializationCompleted)
            {
                await WaitForInitialization();
            }
            else
            {
                if (!IsLoaded)
                {
                    return;
                }
            }
            await packetHandler(param);
        };
    }

    /// <summary>
    ///     Registers a packet handler that is only invoked after the plugin has been initialized.
    /// </summary>
    /// <typeparam name="TPacket">The type of the packet.</typeparam>
    /// <param name="packetHandler">The packet handler to be called.</param>
    /// <param name="queuePacketsSentBeforeInitializationCompleted">Whether packets sent before the plugin has been initialized should be queued and processed after initialization.</param>
    public void OnPacketAfterInitialization<TPacket>(AsyncPacketHandler<TPacket> packetHandler, bool queuePacketsSentBeforeInitializationCompleted = true)
        where TPacket : IPacket
    {
        var registration = Bot.Client.On(CreateAfterInitializationPacketHandlerWrapper(packetHandler, queuePacketsSentBeforeInitializationCompleted));
        if (registration != null)
        {
            PacketReceiveRegistrations.Add(registration);
        }
    }

    internal async Task Initialize()
    {
        if (IsLoaded)
        {
            return;
        }

        try
        {
            await Init();

            initializationTask.TrySetResult();

            Logger.Info("Plugin loaded: {PluginName}", GetType().Name);
        }
        catch (OperationCanceledException e)
        {
            Logger.Error(e, "Plugin {PluginName} was cancelled during Init()", GetType().Name);
            initializationTask.TrySetCanceled(e.CancellationToken);
            throw;
        }
        catch (Exception e)
        {
            Logger.Error(e, "Plugin {PluginName} threw an exception during Init(). Aborting", GetType().Name);
            initializationTask.TrySetException(e);
            throw;
        }
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return DisposeAsyncInternal();
    }

    /// <summary>
    ///     Disposes the plugin asynchronously.
    ///     Can be overridden by plugins to add custom dispose logic.
    ///     IMPORTANT: Always call base.DisposeAsync() in the overridden method (at the end).
    /// </summary>
    /// <returns>A ValueTask representing the asynchronous dispose operation.</returns>
    protected virtual ValueTask DisposeAsyncInternal()
    {
        var registrations = PacketReceiveRegistrations.ToArray();
        foreach (var registration in registrations)
        {
            PacketReceiveRegistrations.TryRemove(registration);
            registration.Dispose();
        }
        return ValueTask.CompletedTask;
    }
}
