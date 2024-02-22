using MineSharp.Auth;
using MineSharp.Bot.Plugins;
using MineSharp.Bot.Proxy;
using MineSharp.Data;
using MineSharp.Protocol;
using NLog;
using System.Net;

namespace MineSharp.Bot;

/// <summary>
/// Builder for <see cref="MineSharpBot"/>
/// </summary>
public class BotBuilder
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private static readonly Type[] DefaultPlugins = [
        typeof(ChatPlugin),
        typeof(EntityPlugin),
        typeof(PlayerPlugin),
        typeof(WindowPlugin),
        typeof(WorldPlugin),
        typeof(CraftingPlugin),
        typeof(PhysicsPlugin)
    ];

    private string? hostname;
    private ushort port = 25565;
    
    // MinecraftData variables
    private string? versionStr;
    private MinecraftData? data;
    private bool autoDetect = true;
    
    // Plugins 
    private List<Type> plugins = [];
    private bool excludeDefaultPlugins = false;
    
    // Session variables
    private Session? session;
    private string? microsoftLoginUsername = null;
    private MicrosoftAuth.DeviceCodeHandler? deviceCodeHandler = null;

    // proxy
    private ProxyFactory? proxyProvider = null;

    private bool autoConnect;

    /// <summary>
    /// Specify the hostname
    /// </summary>
    /// <param name="hostnameOrIpAddress"></param>
    /// <returns></returns>
    public BotBuilder Host(string hostnameOrIpAddress)
    {
        this.hostname = hostnameOrIpAddress;
        return this;
    }

    /// <summary>
    /// Specify the port. Defaults to 25565
    /// </summary>
    /// <param name="port"></param>
    /// <returns></returns>
    public BotBuilder Port(ushort port)
    {
        this.port = port;
        return this;
    }

    /// <summary>
    /// Set MinecraftData by version string
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    public BotBuilder Data(string version)
    {
        this.versionStr = version;
        return this;
    }

    /// <summary>
    /// Set MinecraftData
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public BotBuilder Data(MinecraftData data)
    {
        this.data = data;
        return this;
    }

    /// <summary>
    /// Specify to auto detect MinecraftData from the server
    /// </summary>
    /// <param name="autoDetect"></param>
    /// <returns></returns>
    public BotBuilder AutoDetectData(bool autoDetect)
    {
        this.autoDetect = autoDetect;
        return this;
    }

    /// <summary>
    /// Add a plugin to the bot
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public BotBuilder WithPlugin<T>() where T : Plugin
    {
        this.plugins.Add(typeof(T));
        return this;
    }

    /// <summary>
    /// Do not load the default plugins
    /// </summary>
    /// <returns></returns>
    public BotBuilder ExcludeDefaultPlugins()
    {
        this.excludeDefaultPlugins = true;
        return this;
    }

    /// <summary>
    /// Set the Session
    /// </summary>
    /// <param name="session"></param>
    /// <returns></returns>
    public BotBuilder Session(Session session)
    {
        this.session = session;
        return this;
    }

    /// <summary>
    /// Create a new offline session
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public BotBuilder OfflineSession(string username)
    {
        this.session = Auth.Session.OfflineSession(username);
        return this;
    }

    /// <summary>
    /// Create a new online session
    /// </summary>
    /// <param name="usernameOrEmail"></param>
    /// <param name="deviceCodeHandler"></param>
    /// <returns></returns>
    public BotBuilder OnlineSession(string usernameOrEmail, MicrosoftAuth.DeviceCodeHandler? deviceCodeHandler = null)
    {
        this.microsoftLoginUsername = usernameOrEmail;
        this.deviceCodeHandler = deviceCodeHandler;
        return this;
    }

    /// <summary>
    /// Specify whether to automatically connect when creating the bot
    /// </summary>
    /// <param name="autoConnect"></param>
    /// <returns></returns>
    public BotBuilder AutoConnect(bool autoConnect = true)
    {
        this.autoConnect = autoConnect;
        return this;
    }

    /// <summary>
    /// Use a proxy for minecraft services and Minecraft client
    /// </summary>
    /// <param name="type"></param>
    /// <param name="hostname"></param>
    /// <param name="port"></param>
    /// <returns></returns>
    public BotBuilder WithProxy(ProxyFactory.ProxyType type, string hostname, int port)
    {
        this.proxyProvider = new ProxyFactory(type, hostname, port);
        return this;
    }
    
    /// <summary>
    /// Use a proxy for minecraft services and Minecraft client
    /// </summary>
    /// <param name="type"></param>
    /// <param name="hostname"></param>
    /// <param name="port"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public BotBuilder WithProxy(ProxyFactory.ProxyType type, string hostname, int port, string username, string password)
    {
        this.proxyProvider = new ProxyFactory(type, hostname, port, username, password);
        return this;
    }

    /// <summary>
    /// Create the <see cref="MineSharpBot"/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="Exception"></exception>
    public async Task<MineSharpBot> CreateAsync()
    {
        if (this.hostname is null)
            throw new ArgumentNullException(nameof(this.hostname));

        this.proxyProvider ??= new ProxyFactory();

        MinecraftApi api = new MinecraftApi(this.proxyProvider.CreateHttpClient());
        
        MinecraftData? data;
        if (this.data is not null)
            data = this.data;
        else if (this.versionStr is not null)
            data = await MinecraftData.FromVersion(this.versionStr);
        else if (this.autoDetect)
            data = await TryAutoDetectVersion(this.proxyProvider, this.hostname, this.port);
        else throw new ArgumentNullException(nameof(this.data), "No data provided. Set either Data() or AutoDetectVersion(true)");

        Session? session;
        if (this.session is not null)
            session = this.session;
        else if (this.microsoftLoginUsername is not null)
            session = await MicrosoftAuth.Login(this.microsoftLoginUsername, this.deviceCodeHandler);
        else throw new ArgumentNullException(nameof(this.session), "No session provided. Set either Session(), OfflineSession() or OnlineSession()");

        var client = new MinecraftClient(
            data,
            session,
            this.hostname,
            this.port,
            api,
            this.proxyProvider);
        
        var bot = new MineSharpBot(client);
        
        if (!this.excludeDefaultPlugins)
            this.plugins.AddRange(DefaultPlugins);

        foreach (var type in this.plugins)
        {
            var plugin = (Plugin?)Activator.CreateInstance(type, bot);
            if (plugin is null)
            {
                throw new Exception($"Could not create plugin of type {type.Name}");
            }
            await bot.LoadPlugin(plugin);
        }

        if (this.autoConnect)
            await bot.Connect();

        return bot;
    }

    /// <summary>
    /// Create the <see cref="MineSharpBot"/> synchronously.
    /// Note: May block for some time.
    /// </summary>
    /// <returns></returns>
    public MineSharpBot Create()
        => this.CreateAsync().Result;

    private static async Task<MinecraftData> TryAutoDetectVersion(ProxyFactory factory, string hostname, ushort port)
    {
        var status = await MinecraftClient.RequestServerStatus(hostname, port, tcpFactory: factory);

        if (status.Brand != "Vanilla")
        {
            Logger.Warn($"MineSharp was not tested on server brand '{status.Brand}'");
        }
        
        return await MinecraftData.FromVersion(status.Version);
    }
}
