using MineSharp.Auth;
using MineSharp.Bot.Exceptions;
using MineSharp.Bot.Plugins;
using MineSharp.Core.Common.Protocol;
using MineSharp.Data;
using MineSharp.Protocol;
using NLog;

namespace MineSharp.Bot;

public class MinecraftBot
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    public event Events.BotStringEvent? OnBotDisconnected;

    public readonly MinecraftData Data;
    public readonly MinecraftClient Client;
    public readonly Session Session;

    private readonly IDictionary<Guid, Plugin> _plugins;
    private readonly CancellationTokenSource _cancellation;
    
    private Task? _tickLoop;

    // This field is used for syncing block updates since 1.19.
    internal int SequenceId = 0;

    private MinecraftBot(MinecraftData data, Session session, string hostnameOrIp, ushort port)
    {
        this.Data = data;
        this.Session = session;
        this.Client = new MinecraftClient(data, session, hostnameOrIp, port);

        this._cancellation = new CancellationTokenSource();
        this._plugins = new Dictionary<Guid, Plugin>();
        
        this.Client.OnDisconnected += this.OnClientDisconnected;
    }
    
    public async Task LoadPlugin(Plugin plugin)
    {
        this._plugins.Add(plugin.GetType().GUID, plugin);

        if (this._tickLoop == null)
            return;
        
        await plugin.Initialize();
    }

    public T GetPlugin<T>() where T : Plugin
    {
        if (!this._plugins.TryGetValue(typeof(T).GUID, out var plugin))
        {
            throw new PluginNotLoadedException(
                $"The plugin '{typeof(T).Name}' is not loaded.");
        }

        return (T)plugin;
    }

    public async Task<bool> Connect()
    {
        if (!await this.Client.Connect(GameState.Login))
        {
            return false;
        }

        await this.Client.WaitForGame();

        await Task.WhenAll(
            this._plugins.Values
                .Select(pl => pl.Initialize()));
        
        this._tickLoop = this.TickLoop();

        return true;
    }

    public async Task Disconnect(string reason = "disconnect.quitting")
    {
        if (this._tickLoop is { Status: TaskStatus.Running })
        {
            this._cancellation.Cancel();
            await this._tickLoop!;   
        }
        
        await this.Client.Disconnect(reason);
    }

    private async Task TickLoop()
    {
        while (!this._cancellation.Token.IsCancellationRequested)
        {
            var start = DateTime.Now;

            var tasks = this._plugins.Values
                .Where(plugin => plugin.IsEnabled)
                .Select(plugin => plugin.OnTick())
                .ToArray();
            
            await Task.WhenAll(tasks);

            var errors = tasks.Where(x => x.Exception != null);
            foreach (var err in errors)
                Logger.Error($"Error in Module: {err.Exception}");

            var deltaTime = 50 - (int)(DateTime.Now - start).TotalMilliseconds;
            if (deltaTime < 0)
                Logger.Warn($"Ticked modules taking too long, {-deltaTime}ms behind");
            else
                await Task.Delay(deltaTime);
        }
    }

    private void OnClientDisconnected(MinecraftClient sender, string reason)
        => this.OnBotDisconnected?.Invoke(this, reason);


    public static async Task<MinecraftBot> CreateBot(
        string hostnameOrIp,
        Session session,
        ushort port = 25565,
        string? version = null,
        bool excludeDefaultPlugins = false)
    {
        var data = version switch {
            null => await MinecraftClient.AutodetectServerVersion(hostnameOrIp, port),
            _ => MinecraftData.FromVersion(version)
        };

        var bot = new MinecraftBot(data, session, hostnameOrIp, port);

        if (excludeDefaultPlugins)
            return bot;

        Plugin[] defaultPlugins = {
            new ChatPlugin(bot),
            new EntityPlugin(bot),
            new PlayerPlugin(bot),
            new WindowPlugin(bot),
            new WorldPlugin(bot),
            new CraftingPlugin(bot),
            new PhysicsPlugin(bot)
        };

        foreach (var plugin in defaultPlugins)
            await bot.LoadPlugin(plugin);

        return bot;
    }
    
    /// <summary>
    /// Creates a new MinecraftBot.
    /// If you want an online session and login with an Microsoft Account, use your account email as username parameter.
    /// </summary>
    /// <param name="username">The Username of the Bot (offline session), or an microsoft account email (online session)</param>
    /// <param name="hostnameOrIp">The Hostname of the Minecraft server.</param>
    /// <param name="port">Port of the Minecraft server</param>
    /// <param name="offline">When true, you won't be logged in to the minecraft services, and will only be able to join servers in offline-mode.</param>
    /// <param name="version">The minecraft version to use. If null, MineSharp will try to automatically detect the version.</param>
    /// <param name="excludeDefaultPlugins">When true, the default plugins will not be added to the bot</param>
    /// <returns></returns>
    public static async Task<MinecraftBot> CreateBot(
        string username, 
        string hostnameOrIp, 
        ushort port = 25565,
        bool offline = false, 
        string? version = null,
        bool excludeDefaultPlugins = false)
    {
        var session = offline switch {
            true => Session.OfflineSession(username),
            false => await MicrosoftAuth.Login(username)
        };

        return await CreateBot(hostnameOrIp, session, port, version, excludeDefaultPlugins);
    }
}