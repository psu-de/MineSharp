using MineSharp.Protocol.Packets;
using NLog;
using static MineSharp.Protocol.MinecraftClient;

namespace MineSharp.Bot.Plugins;

/// <summary>
///     Plugin for <see cref="MineSharpBot" />.
/// </summary>
public abstract class Plugin
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private readonly TaskCompletionSource initializationTask;

    /// <summary>
    ///     Create a new Plugin instance
    /// </summary>
    /// <param name="bot"></param>
    protected Plugin(MineSharpBot bot)
    {
        Bot = bot;
        IsEnabled = true;
        initializationTask = new();
    }

    /// <summary>
    ///     The bot
    /// </summary>
    protected MineSharpBot Bot { get; }

    /// <summary>
    ///     Whether this plugin is currently enabled
    /// </summary>
    public bool IsEnabled { get; private set; }

    /// <summary>
    ///     Whether this plugin is loaded and functional
    /// </summary>
    public bool IsLoaded { get; private set; }

    /// <summary>
    ///     This method is called once when the plugin starts.
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
    public virtual Task OnTick()
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

    private AsyncPacketHandler<TPacket> CreateAfterInitializationPacketHandlerWrapper<TPacket>(AsyncPacketHandler<TPacket> packetHandler)
        where TPacket : IPacket
    {
        return async param =>
        {
            await WaitForInitialization();
            await packetHandler(param);
        };
    }

    public void OnPacketAfterInitialization<TPacket>(AsyncPacketHandler<TPacket> packetHandler)
        where TPacket : IPacket
    {
        Bot.Client.On(CreateAfterInitializationPacketHandlerWrapper(packetHandler));
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

            IsLoaded = true;
            Logger.Info("Plugin loaded: {PluginName}", GetType().Name);
        }
        catch (Exception e)
        {
            Logger.Error(e, "Plugin {PluginName} threw an exception during Init(). Aborting", GetType().Name);
            throw;
        }
    }
}
