using fNbt;
using MineSharp.Auth;
using MineSharp.Bot.Exceptions;
using MineSharp.Bot.Plugins;
using MineSharp.Core.Common.Protocol;
using MineSharp.Data;
using MineSharp.Protocol;
using MineSharp.Protocol.Packets.Clientbound.Configuration;
using MineSharp.Protocol.Packets.Clientbound.Play;
using NLog;

namespace MineSharp.Bot;

/// <summary>
/// A Minecraft Bot.
/// The Minecraft Bot uses Plugins that contain helper methods to handle and send minecraft packets.
/// </summary>
public class MineSharpBot
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Fired when the bot disconnects
    /// </summary>
    public event Events.BotStringEvent? OnBotDisconnected;

    /// <summary>
    /// The <see cref="MinecraftData"/> instance used by this bot
    /// </summary>
    public readonly MinecraftData Data;

    /// <summary>
    /// The underlying <see cref="MinecraftClient"/> used by this bot
    /// </summary>
    public readonly MinecraftClient Client;

    /// <summary>
    /// The <see cref="Session"/> object used by this bot
    /// </summary>
    public readonly Session Session;

    /// <summary>
    /// NBT Registry sent by the server
    /// </summary>
    public NbtCompound Registry = [];

    private readonly IDictionary<Guid, Plugin> _plugins;
    private readonly CancellationTokenSource   _cancellation;

    private Task? _tickLoop;

    // This field is used for syncing block updates since 1.19.
    internal int SequenceId = 0;

    /// <summary>
    /// Create a new MineSharpBot instance with a <see cref="MinecraftClient"/>
    /// </summary>
    /// <param name="client"></param>
    public MineSharpBot(MinecraftClient client)
    {
        this.Client  = client;
        this.Data    = this.Client.Data;
        this.Session = this.Client.Session;

        this._cancellation = new CancellationTokenSource();
        this._plugins      = new Dictionary<Guid, Plugin>();

        this.Client.OnDisconnected += this.OnClientDisconnected;
        this.Client.On<RegistryDataPacket>(packet => Task.FromResult(this.Registry = packet.RegistryData));
        this.Client.On<LoginPacket>(packet => Task.FromResult(packet.RegistryCodec != null ? this.Registry = packet.RegistryCodec : null));
    }

    /// <summary>
    /// Load the given plugin and initialize it when the bot is already connected
    /// </summary>
    /// <param name="plugin"></param>
    public async Task LoadPlugin(Plugin plugin)
    {
        this._plugins.Add(plugin.GetType().GUID, plugin);

        if (this._tickLoop == null)
            return;

        await plugin.Initialize();
    }

    /// <summary>
    /// Get the plugin of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="PluginNotLoadedException"></exception>
    public T GetPlugin<T>() where T : Plugin
    {
        if (!this._plugins.TryGetValue(typeof(T).GUID, out var plugin))
        {
            throw new PluginNotLoadedException(
                $"The plugin '{typeof(T).Name}' is not loaded.");
        }

        return (T)plugin;
    }

    /// <summary>
    /// Connect to the Minecraft server
    /// </summary>
    /// <returns>True when connected successfully and finished the handshake</returns>
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

    /// <summary>
    /// Disconnect from the Minecraft server
    /// </summary>
    /// <param name="reason">The reason for disconnecting</param>
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
}
