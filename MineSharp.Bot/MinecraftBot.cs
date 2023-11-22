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

    public MinecraftData Data { get; }
    public MinecraftClient Client { get; }
    public Session Session { get; }
    
    public ChatPlugin? Chat { get; private set; }
    public EntityPlugin? Entities { get; private set; }
    public PlayerPlugin? Player { get; private set; }
    public WorldPlugin? World { get; private set; }
    public WindowPlugin? Windows { get; private set; }

    private readonly IDictionary<Guid, Plugin> _plugins;
    private readonly CancellationTokenSource _cancellation;
    
    private Task? _tickLoop;

    internal int SequenceId;

    private MinecraftBot(MinecraftData data, Session session, string hostnameOrIp, ushort port)
    {
        this.Data = data;
        this.Session = session;
        this.Client = new MinecraftClient(data, session, hostnameOrIp, port);

        this._cancellation = new CancellationTokenSource();
        this._plugins = new Dictionary<Guid, Plugin>();
        this.AddDefaultPlugins();

        this.Client.OnDisconnected += this.OnClientDisconnected;
    }
    
    public async Task LoadPlugin(Plugin plugin)
    {
        if (this._tickLoop == null)
        {
            this._plugins.Add(plugin.GetType().GUID, plugin);
            return;
        }

        await plugin.Initialize();
        this._plugins.Add(plugin.GetType().GUID, plugin);
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
        
        await this.InitializePlugins();
        this._tickLoop = this.TickLoop();

        return true;
    }

    public async Task Disconnect(string reason = "disconnect.quitting")
    {
        if (this._tickLoop != null && this._tickLoop.Status == TaskStatus.Running)
        {
            this._cancellation.Cancel();
            await this._tickLoop!;   
        }
        
        await this.Client.Disconnect(reason);
    }

    private void AddDefaultPlugins()
    {
        void AddPlugin(Plugin plugin) 
            => this._plugins.Add(plugin.GetType().GUID, plugin);

        this.Player = new PlayerPlugin(this);
        this.Entities = new EntityPlugin(this);
        this.World = new WorldPlugin(this);
        this.Chat = new ChatPlugin(this);
        this.Windows = new WindowPlugin(this);

        AddPlugin(this.Player);
        AddPlugin(this.Entities);
        AddPlugin(this.World);
        AddPlugin(this.Chat);
        AddPlugin(this.Windows);
    }
    
    private Task InitializePlugins()
    {
        return Task.WhenAll(
            this._plugins.Values.Select(pl => pl.Initialize()));
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
    
    
    public static async Task<MinecraftBot> CreateBot(string username, string hostnameOrIp, ushort port, bool offline = false, string? version = null)
    {
        var session = offline switch {
            true => Session.OfflineSession(username),
            false => await Session.Login(username)
        };
        
        var data = version switch {
            null => await MinecraftClient.AutodetectServerVersion(hostnameOrIp, port),
            _ => MinecraftData.FromVersion(version)
        };

        return new MinecraftBot(data, session, hostnameOrIp, port);
    }
}