using MineSharp.Auth;
using MineSharp.Bot.Plugins;
using MineSharp.Bot.Proxy;
using MineSharp.Data;
using MineSharp.Protocol;
using MineSharp.Protocol.Connection;
using NLog;

namespace MineSharp.Bot;

/// <summary>
///     Builder for <see cref="MineSharpBot" />
/// </summary>
public class BotBuilder
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private static readonly Type[] DefaultPlugins =
    [
        typeof(ChatPlugin),
        typeof(EntityPlugin),
        typeof(PlayerPlugin),
        typeof(WindowPlugin),
        typeof(WorldPlugin),
        typeof(CraftingPlugin),
        typeof(PhysicsPlugin)
    ];

    private readonly Context ctx = new();
    
    /// <summary>
    ///     Specify the hostname
    /// </summary>
    /// <param name="hostnameOrIpAddress"></param>
    /// <returns></returns>
    public BotBuilder Host(string hostnameOrIpAddress)
    {
        ctx.Connection.Hostname = hostnameOrIpAddress;
        return this;
    }

    /// <summary>
    ///     Specify the port. Defaults to 25565
    /// </summary>
    /// <param name="port"></param>
    /// <returns></returns>
    public BotBuilder Port(ushort port)
    {
        ctx.Connection.Port = port;
        return this;
    }

    /// <summary>
    ///     Set MinecraftData by version string
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    public BotBuilder Data(string version)
    {
        ctx.Data.Version = version;
        return this;
    }

    /// <summary>
    ///     Set MinecraftData
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public BotBuilder Data(MinecraftData data)
    {
        ctx.Data.Data = data;
        return this;
    }

    /// <summary>
    ///     Specify to auto detect MinecraftData from the server
    /// </summary>
    /// <param name="autoDetect"></param>
    /// <returns></returns>
    public BotBuilder AutoDetectData(bool autoDetect)
    {
        ctx.Data.AutoDetect = autoDetect;
        return this;
    }

    /// <summary>
    ///     Add a plugin to the bot
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public BotBuilder WithPlugin<T>() where T : Plugin
    {
        ctx.Settings.Plugins.Add(typeof(T));
        return this;
    }

    /// <summary>
    ///     Set the client settings
    /// </summary>
    public BotBuilder WithSettings(ClientSettings settings)
    {
        ctx.Settings.Settings = settings;
        return this;
    }

    /// <summary>
    ///     Do not load the default plugins
    /// </summary>
    /// <returns></returns>
    public BotBuilder ExcludeDefaultPlugins()
    {
        ctx.Settings.ExcludeDefaultPlugins = true;
        return this;
    }

    /// <summary>
    ///     Set the Session
    /// </summary>
    /// <param name="session"></param>
    /// <returns></returns>
    public BotBuilder Session(Session session)
    {
        ctx.Session.Session = session;
        return this;
    }

    /// <summary>
    ///     Create a new offline session
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public BotBuilder OfflineSession(string username)
    {
        ctx.Session.Session = Auth.Session.OfflineSession(username);
        return this;
    }

    /// <summary>
    ///     Create a new online session
    /// </summary>
    /// <param name="usernameOrEmail"></param>
    /// <param name="deviceCodeHandler"></param>
    /// <returns></returns>
    public BotBuilder OnlineSession(string usernameOrEmail, MicrosoftAuth.DeviceCodeHandler? deviceCodeHandler = null)
    {
        ctx.Session.Username          = usernameOrEmail;
        ctx.Session.DeviceCodeHandler = deviceCodeHandler;
        return this;
    }

    /// <summary>
    ///     Specify whether to automatically connect when creating the bot
    /// </summary>
    /// <param name="autoConnect"></param>
    /// <returns></returns>
    public BotBuilder AutoConnect(bool autoConnect = true)
    {
        ctx.Connection.AutoConnect = autoConnect;
        return this;
    }

    /// <summary>
    ///     Use a proxy for minecraft services and Minecraft client
    /// </summary>
    /// <param name="type"></param>
    /// <param name="hostname"></param>
    /// <param name="port"></param>
    /// <returns></returns>
    public BotBuilder WithProxy(ProxyFactory.ProxyType type, string hostname, int port)
    {
        ctx.Connection.ProxyProvider = new ProxyFactory(type, hostname, port);
        return this;
    }

    /// <summary>
    ///     Use a proxy for minecraft services and Minecraft client
    /// </summary>
    /// <param name="type"></param>
    /// <param name="hostname"></param>
    /// <param name="port"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public BotBuilder WithProxy(ProxyFactory.ProxyType type, string hostname, int port, string username,
                                string password)
    {
        ctx.Connection.ProxyProvider = new ProxyFactory(type, hostname, port, username, password);
        return this;
    }

    /// <summary>
    ///     Create the <see cref="MineSharpBot" />
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="Exception"></exception>
    public async Task<MineSharpBot> CreateAsync()
    {
        var client = new MinecraftClient(
            await ctx.Data.Resolve(ctx.Connection),
            await ctx.Session.Resolve(ctx.Connection),
            ctx.Connection.Hostname,
            ctx.Connection.Port,
            ctx.Connection.Api,
            ctx.Connection.ProxyProvider,
            ctx.Settings.Settings);

        var bot = new MineSharpBot(client);

        if (!ctx.Settings.ExcludeDefaultPlugins)
        {
            ctx.Settings.Plugins.AddRange(DefaultPlugins);
        }

        foreach (var type in ctx.Settings.Plugins)
        {
            var plugin = (Plugin?)Activator.CreateInstance(type, bot);
            if (plugin is null)
            {
                throw new NullReferenceException($"failed to create instance of plugin {type.Name}");
            }

            await bot.LoadPlugin(plugin);
        }

        if (ctx.Connection.AutoConnect)
        {
            await bot.Connect();
        }

        return bot;
    }

    /// <summary>
    ///     Create the <see cref="MineSharpBot" /> synchronously.
    ///     Note: May block for some time.
    /// </summary>
    /// <returns></returns>
    public MineSharpBot Create()
    {
        return CreateAsync().Result;
    }

    private static async Task<MinecraftData> TryAutoDetectVersion(IConnectionFactory factory, string hostname, ushort port)
    {
        var status = await MinecraftClient.RequestServerStatus(hostname, port, tcpFactory: factory);

        if (status.Brand != "Vanilla")
        {
            Logger.Warn($"MineSharp was not tested on server brand '{status.Brand}'");
        }

        return await MinecraftData.FromVersion(status.Version);
    }

    private class Context
    {
        public readonly ConnectionCtx Connection = new();
        public readonly SessionCtx    Session    = new();
        public readonly DataCtx       Data       = new();
        public readonly SettingsCtx   Settings   = new();
        
        public class ConnectionCtx
        {
            public string             Hostname      { get; set; } = "localhost";
            public ushort             Port          { get; set; } = 25565;
            public bool               AutoConnect   { get; set; } = false;
            public MinecraftApi       Api           { get; set; } = MinecraftApi.Instance;

            public IConnectionFactory ProxyProvider
            {
                get => connectionFactory;
                set
                {
                    connectionFactory = value;
                    Api               = new (connectionFactory.CreateHttpClient());
                }
            }
            
            private IConnectionFactory connectionFactory = DefaultTcpFactory.Instance;
        }

        public class SessionCtx
        {
            public Session?                         Session           { get; set; } = null;
            public string?                          Username          { get; set; } = null;
            public MicrosoftAuth.DeviceCodeHandler? DeviceCodeHandler { get; set; } = null;

            public Task<Session> Resolve(ConnectionCtx ctx)
            {
                if (Session is not null)
                {
                    return Task.FromResult(Session);
                }

                if (Username is null)
                {
                    throw new ArgumentException("no session or username provided");
                }

                return MicrosoftAuth.Login(Username, DeviceCodeHandler, ctx.Api);
            }
        }
        
        public class DataCtx
        {
            public bool           AutoDetect  { get; set; } = true;
            public string?        Version     { get; set; } = null; 
            public MinecraftData? Data        { get; set; } = null;

            public Task<MinecraftData> Resolve(ConnectionCtx connCtx)
            {
                if (Data is not null)
                {
                    return Task.FromResult(Data);
                }

                if (Version is not null)
                {
                    return MinecraftData.FromVersion(Version);
                }

                if (!AutoDetect)
                {
                    throw new ArgumentException(
                        $"{nameof(AutoDetect)} is false and neither {nameof(Data)} nor {nameof(Version)} is specified.");
                }

                return TryAutoDetectVersion(connCtx);
            }

            private static async Task<MinecraftData> TryAutoDetectVersion(ConnectionCtx ctx)
            {
                var status = await MinecraftClient.RequestServerStatus(ctx.Hostname, ctx.Port, tcpFactory: ctx.ProxyProvider);

                if (status.Brand != "Vanilla")
                {
                    Logger.Warn($"MineSharp was not tested on server brand '{status.Brand}'");
                }

                return await MinecraftData.FromVersion(status.Version);
            }
        }

        public class SettingsCtx
        {
            public ClientSettings Settings              { get; set; } = ClientSettings.Default;
            public bool           ExcludeDefaultPlugins { get; set; } = false;
            public List<Type>     Plugins               { get; set; } = [];
        }
    }
}
