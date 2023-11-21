using MineSharp.Protocol;

namespace MineSharp.Bot.Plugins;

public abstract class Plugin
{
    protected MinecraftBot Bot { get; }

    public bool IsEnabled { get; private set; }
    public bool IsLoaded { get; private set; }

    private readonly TaskCompletionSource _initializationTask;

    protected Plugin(MinecraftBot bot)
    {
        this.Bot = bot;
        this.IsEnabled = true;
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
    }
}
