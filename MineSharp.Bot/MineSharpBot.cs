using fNbt;
using MineSharp.Auth;
using MineSharp.Bot.Exceptions;
using MineSharp.Bot.Plugins;
using MineSharp.Core.Common.Protocol;
using MineSharp.Core.Concurrency;
using MineSharp.Data;
using MineSharp.Protocol;
using MineSharp.Protocol.Packets.Clientbound.Configuration;
using MineSharp.Protocol.Packets.Clientbound.Play;
using NLog;

namespace MineSharp.Bot;

/// <summary>
///     A Minecraft Bot.
///     The Minecraft Bot uses Plugins that contain helper methods to handle and send minecraft packets.
/// </summary>
public sealed class MineSharpBot : IAsyncDisposable, IDisposable
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    ///     The underlying <see cref="MinecraftClient" /> used by this bot
    /// </summary>
    public readonly MinecraftClient Client;

    /// <summary>
    ///     Is cancelled once the client needs to stop. Usually because the connection was lost.
    /// </summary>
    public CancellationToken CancellationToken => Client!.CancellationToken;

    /// <summary>
    ///     The <see cref="MinecraftData" /> instance used by this bot
    /// </summary>
    public readonly MinecraftData Data;

    private readonly IDictionary<Guid, Plugin> plugins;

    /// <summary>
    ///     The <see cref="Session" /> object used by this bot
    /// </summary>
    public readonly Session Session;

    /// <summary>
    ///     NBT Registry sent by the server
    /// </summary>
    public NbtCompound Registry { get; private set; } = [];

    // This field is used for syncing block updates since 1.19.
    internal int SequenceId = 0;

    private Task? tickLoop;

    /// <summary>
    ///     Create a new MineSharpBot instance with a <see cref="MinecraftClient" />
    /// </summary>
    /// <param name="client"></param>
    public MineSharpBot(MinecraftClient client)
    {
        Client = client;
        Data = Client.Data;
        Session = Client.Session;

        plugins = new Dictionary<Guid, Plugin>();

        Client.On<RegistryDataPacket>(packet => Task.FromResult(Registry = packet.RegistryData));
        Client.On<LoginPacket>(
            packet => Task.FromResult(packet.RegistryCodec != null ? Registry = packet.RegistryCodec : null));
    }

    /// <summary>
    ///     Load the given plugin and initialize it when the bot is already connected
    /// </summary>
    /// <param name="plugin"></param>
    public async Task LoadPlugin(Plugin plugin)
    {
        plugins.Add(plugin.GetType().GUID, plugin);

        if (tickLoop == null)
        {
            return;
        }

        await plugin.Initialize();
    }

    /// <summary>
    ///     Get the plugin of type <typeparamref name="T" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="PluginNotLoadedException"></exception>
    public T GetPlugin<T>() where T : Plugin
    {
        if (!plugins.TryGetValue(typeof(T).GUID, out var plugin))
        {
            throw new PluginNotLoadedException(
                $"The plugin '{typeof(T).Name}' is not loaded.");
        }

        return (T)plugin;
    }

    /// <summary>
    ///     Connect to the Minecraft server
    /// </summary>
    /// <returns>True when connected successfully and finished the handshake</returns>
    public async Task<bool> Connect()
    {
        if (!await Client.Connect(GameState.Login))
        {
            return false;
        }

        await Client.WaitForGame();

        await Task.WhenAll(
            plugins.Values
                   .Select(pl => pl.Initialize()));

        tickLoop = Task.Run(TickLoop);

        return true;
    }

    /// <summary>
    ///     Disconnect from the Minecraft server
    /// </summary>
    /// <param name="reason">The reason for disconnecting</param>
    public EnsureOnlyRunOnceAsyncResult Disconnect(ChatComponent.Chat? reason = null)
    {
        // there is no point in waiting for the tickLoop
        // it wil be cancelled when the bot disconnects

        return Client.Disconnect(reason);
    }

    private async Task TickLoop()
    {
        while (!CancellationToken.IsCancellationRequested)
        {
            var start = DateTime.Now;

            var tasks = plugins.Values
                               .Where(plugin => plugin.IsEnabled)
                               .Select(plugin => plugin.OnTick())
                               .ToArray();

            await Task.WhenAll(tasks);

            var errors = tasks.Where(x => x.Exception != null);
            foreach (var err in errors)
            {
                Logger.Error($"Error in Module: {err.Exception}");
            }

            var deltaTime = 50 - (int)(DateTime.Now - start).TotalMilliseconds;
            if (deltaTime < 0)
            {
                Logger.Warn($"Ticked modules taking too long, {-deltaTime}ms behind");
            }
            else
            {
                await Task.Delay(deltaTime);
            }
        }
    }

    public void Dispose()
    {
        // TODO: improve packet callback registrations to be able to remove them here
    }

    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }
}
