using MineSharp.Protocol;
using NLog;

namespace MineSharp.Bot.Plugins;

/// <summary>
/// Plugin for <see cref="MineSharpBot"/>. 
/// </summary>
public abstract class Plugin
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// The bot
    /// </summary>
    protected MineSharpBot Bot { get; }

    /// <summary>
    /// Whether this plugin is currently enabled
    /// </summary>
    public bool IsEnabled { get; private set; }

    /// <summary>
    /// Whether this plugin is loaded and functional
    /// </summary>
    public bool IsLoaded { get; private set; }

    private readonly TaskCompletionSource _initializationTask;

    /// <summary>
    /// Create a new Plugin instance
    /// </summary>
    /// <param name="bot"></param>
    protected Plugin(MineSharpBot bot)
    {
        this.Bot                 = bot;
        this.IsEnabled           = true;
        this._initializationTask = new TaskCompletionSource();
    }

    /// <summary>
    /// This method is called once when the plugin starts.
    /// </summary>
    /// <returns></returns>
    protected virtual Task Init()
        => Task.CompletedTask;

    /// <summary>
    /// This method is called each minecraft tick. (About 20 times a second.)
    /// </summary>
    /// <returns></returns>
    public virtual Task OnTick()
        => Task.CompletedTask;

    /// <summary>
    /// This method is called when the plugin gets enabled.
    /// </summary>
    /// <returns></returns>
    protected virtual Task OnEnable()
        => Task.CompletedTask;

    /// <summary>
    /// This method is called when the plugin gets disabled.
    /// </summary>
    /// <returns></returns>
    protected virtual Task OnDisable()
        => Task.CompletedTask;

    /// <summary>
    /// Enable or disable the plugin.
    /// </summary>
    /// <param name="enabled"></param>
    public async Task SetEnabled(bool enabled)
    {
        if (this.IsEnabled == enabled)
            return;

        if (enabled)
            await this.OnEnable();
        else
            await this.OnDisable();

        this.IsEnabled = enabled;
    }

    /// <summary>
    /// Returns a task that resolves once this module has been initialized.
    /// </summary>
    /// <returns></returns>
    public Task WaitForInitialization()
    {
        return this._initializationTask.Task;
    }

    internal async Task Initialize()
    {
        if (this.IsLoaded)
            return;

        await this.Init();
        this._initializationTask.TrySetResult();

        this.IsLoaded = true;
        Logger.Info("Plugin loaded: {PluginName}", this.GetType().Name);
    }
}
