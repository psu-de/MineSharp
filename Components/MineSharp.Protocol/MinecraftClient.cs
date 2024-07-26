using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using MineSharp.Auth;
using MineSharp.ChatComponent;
using MineSharp.ChatComponent.Components;
using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Protocol;
using MineSharp.Core.Events;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Channels;
using MineSharp.Protocol.Connection;
using MineSharp.Protocol.Exceptions;
using MineSharp.Protocol.Packets;
using MineSharp.Protocol.Packets.Clientbound.Status;
using MineSharp.Protocol.Packets.Handlers;
using MineSharp.Protocol.Packets.Serverbound.Configuration;
using MineSharp.Protocol.Packets.Serverbound.Status;
using Newtonsoft.Json.Linq;
using NLog;

namespace MineSharp.Protocol;

/// <summary>
///     A Minecraft client.
///     Connect to a minecraft server.
/// </summary>
public sealed class MinecraftClient : IDisposable
{
    /// <summary>
    ///     Delegate for handling packets async
    /// </summary>
    public delegate Task AsyncPacketHandler(IPacket packet);

    /// <summary>
    ///     Delegate for handling a specific packet async
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public delegate Task AsyncPacketHandler<in T>(T packet) where T : IPacket;

    /// <summary>
    ///     The latest version supported
    /// </summary>
    public const string LATEST_SUPPORTED_VERSION = "1.20.4";

    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    internal readonly MinecraftApi? Api;
    private readonly Queue<(PacketType, PacketBuffer)> bundledPackets;
    private readonly CancellationTokenSource cancellationTokenSource;

    /// <summary>
    ///     The MinecraftData object of this client
    /// </summary>
    public readonly MinecraftData Data;

    private readonly TaskCompletionSource gameJoinedTsc;

    /// <summary>
    ///     The Hostname of the minecraft server provided in the constructor
    /// </summary>
    public readonly string Hostname;

    private readonly IPAddress ip;
    private readonly IDictionary<PacketType, IList<AsyncPacketHandler>> packetHandlers;
    private readonly ConcurrentQueue<PacketSendTask> packetQueue;
    private readonly IDictionary<PacketType, TaskCompletionSource<object>> packetWaiters;

    /// <summary>
    ///     The Port of the minecraft server
    /// </summary>
    public readonly ushort Port;

    /// <summary>
    ///     The Session object for this client
    /// </summary>
    public readonly Session Session;

    /// <summary>
    ///     The clients settings
    /// </summary>
    public readonly ClientSettings Settings;
    
    /// <summary>
    ///     Fires when the client disconnected from the server
    /// </summary>
    public AsyncEvent<MinecraftClient, Chat> OnDisconnected = new();

    /// <summary>
    ///     The plugin channels
    ///     See https://wiki.vg/Plugin_channels#Definitions
    /// </summary>
    public readonly PluginChannels Channels;

    private readonly IConnectionFactory tcpTcpFactory;

    private bool bundlePackets;
    private TcpClient? client;

    private IPacketHandler internalPacketHandler;
    private MinecraftStream? stream;
    private Task? streamLoop;
    
    internal GameState GameState;

    /// <summary>
    ///     Create a new MinecraftClient
    /// </summary>
    public MinecraftClient(
        MinecraftData data,
        Session session,
        string hostnameOrIp,
        ushort port,
        MinecraftApi api,
        IConnectionFactory tcpFactory,
        ClientSettings settings)
    {
        Data = data;
        packetQueue = new();
        cancellationTokenSource = new();
        internalPacketHandler = new HandshakePacketHandler(this);
        packetHandlers = new Dictionary<PacketType, IList<AsyncPacketHandler>>();
        packetWaiters = new Dictionary<PacketType, TaskCompletionSource<object>>();
        gameJoinedTsc = new();
        bundledPackets = new();
        tcpTcpFactory = tcpFactory;
        ip = IpHelper.ResolveHostname(hostnameOrIp, ref port);

        Api       = api;
        Session   = session;
        Port      = port;
        Hostname  = hostnameOrIp;
        GameState = GameState.Handshaking;
        Settings  = settings;
        Channels  = new (this);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        cancellationTokenSource.Cancel();
        streamLoop?.Wait();

        client?.Dispose();
        stream?.Close();
    }

    /// <summary>
    ///     Connects to the minecraft sever.
    /// </summary>
    /// <param name="nextState">The state to transition to during the handshaking process.</param>
    /// <returns>A task that resolves once connected. Results in true when connected successfully, false otherwise.</returns>
    public async Task<bool> Connect(GameState nextState)
    {
        if (client is not null && client.Connected)
        {
            Logger.Warn("Client is already connected!");
            return true;
        }

        Logger.Debug($"Connecting to {ip}:{Port}.");

        try
        {
            client = await tcpTcpFactory.CreateOpenConnection(ip, Port);
            stream = new(client.GetStream(), Data.Version.Protocol);

            StreamLoop();
            Logger.Info("Connected, starting handshake...");
            await HandshakeProtocol.PerformHandshake(this, nextState, Data);
        }
        catch (SocketException ex)
        {
            Logger.Error(ex, "Error while connecting");
            return false;
        }

        return true;
    }

    /// <summary>
    ///     Sends a Packet to the Minecraft Server
    /// </summary>
    /// <param name="packet">The packet to send.</param>
    /// <param name="cancellation">Optional cancellation token.</param>
    /// <returns>A task that resolves once the packet was actually sent.</returns>
    public Task SendPacket(IPacket packet, CancellationToken cancellation = default)
    {
        var sendingTask = new PacketSendTask(packet, cancellation, new());
        packetQueue.Enqueue(sendingTask);

        return sendingTask.Task.Task;
    }

    /// <summary>
    ///     Disconnects the client from the server.
    /// </summary>
    /// <param name="reason">The reason the client disconnected. Only used for the <see cref="OnDisconnected" /> event.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task Disconnect(Chat? reason = null)
    {
        reason ??= new TranslatableComponent("disconnect.quitting");
        
        Logger.Info($"Disconnecting: {reason.GetMessage(this.Data)}");

        if (!gameJoinedTsc.Task.IsCompleted)
        {
            gameJoinedTsc.SetException(new DisconnectedException("Client has been disconnected", reason.GetMessage(this.Data)));
        }

        if (client is null || !client.Connected)
        {
            Logger.Warn("Disconnect() was called but client is not connected");
        }

        cancellationTokenSource.Cancel();
        await (streamLoop ?? Task.CompletedTask);

        client?.Close();
        await OnDisconnected.Dispatch(this, reason);
    }

    /// <summary>
    ///     Registers an <paramref name="handler" /> that will be called whenever an packet of type <typeparamref name="T" />
    ///     is received
    /// </summary>
    /// <param name="handler">A delegate that will be called when a packet of type T is received</param>
    /// <typeparam name="T">The type of the packet</typeparam>
    public void On<T>(AsyncPacketHandler<T> handler) where T : IPacket
    {
        var key = PacketPalette.GetPacketType<T>();

        if (!packetHandlers.TryGetValue(key, out var handlers))
        {
            handlers = new List<AsyncPacketHandler>();
            packetHandlers.Add(key, handlers);
        }

        handlers.Add(p => handler((T)p));
    }

    /// <summary>
    ///     Waits until a packet of the specified type is received.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>A task that completes once the packet is received</returns>
    public Task<T> WaitForPacket<T>() where T : IPacket
    {
        var packetType = PacketPalette.GetPacketType<T>();
        if (!packetWaiters.TryGetValue(packetType, out var task))
        {
            var tsc = new TaskCompletionSource<object>();
            packetWaiters.Add(packetType, tsc);

            return tsc.Task.ContinueWith(prev => (T)prev.Result);
        }

        return task.Task.ContinueWith(prev => (T)prev.Result);
    }

    /// <summary>
    ///     Waits until the client jumps into the Play <see cref="GameState" />
    /// </summary>
    /// <returns></returns>
    public Task WaitForGame()
    {
        return gameJoinedTsc.Task;
    }

    internal void UpdateGameState(GameState next)
    {
        GameState = next;

        internalPacketHandler = next switch
        {
            GameState.Handshaking => new HandshakePacketHandler(this),
            GameState.Login => new LoginPacketHandler(this, Data),
            GameState.Status => new StatusPacketHandler(this),
            GameState.Configuration => new ConfigurationPacketHandler(this, Data),
            GameState.Play => new PlayPacketHandler(this, Data),
            _ => throw new UnreachableException()
        };

        if (next == GameState.Play && !gameJoinedTsc.Task.IsCompleted)
        {
            if (Data.Version.Protocol <= ProtocolVersion.V_1_20)
            {
                Task.Delay(10)
                    .ContinueWith(x =>
                                      SendPacket(new Packets.Serverbound.Play.ClientInformationPacket(
                                                     Settings.Locale,
                                                     Settings.ViewDistance,
                                                     (int)Settings.ChatMode,
                                                     Settings.ColoredChat,
                                                     Settings.DisplayedSkinParts,
                                                     (int)Settings.MainHand,
                                                     Settings.EnableTextFiltering,
                                                     Settings.AllowServerListings)));
            }
            gameJoinedTsc.TrySetResult();
        }

        if (next == GameState.Configuration)
        {
            SendPacket(new ClientInformationPacket(
                           Settings.Locale,
                           Settings.ViewDistance,
                           (int)Settings.ChatMode,
                           Settings.ColoredChat,
                           Settings.DisplayedSkinParts,
                           (int)Settings.MainHand,
                           Settings.EnableTextFiltering,
                           Settings.AllowServerListings));
        }
    }

    internal void EnableEncryption(byte[] key)
    {
        stream!.EnableEncryption(key);
    }

    internal void SetCompression(int threshold)
    {
        stream!.SetCompression(threshold);
    }

    internal void HandleBundleDelimiter()
    {
        bundlePackets = !bundlePackets;
        if (!bundlePackets)
        {
            Logger.Debug("Processing bundled packets");
            var tasks = bundledPackets.Select(
                                           p => HandleIncomingPacket(p.Item1, p.Item2, false))
                                      .ToArray();

            Task.WaitAll(tasks);

            var errors = tasks.Where(x => x.Exception != null);
            foreach (var error in errors)
            {
                Logger.Error("Error handling bundled packet: {e}", error);
            }

            bundledPackets.Clear();
        }
        else
        {
            Logger.Debug("Bundling packets!");
        }
    }

    private void StreamLoop()
    {
        streamLoop = Task.Run(async () =>
        {
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    await ReceivePackets();
                    await SendPackets();

                    await Task.Delay(1);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Encountered error in stream loop");
                }
            }
        }, cancellationTokenSource.Token);
    }

    private async Task ReceivePackets()
    {
        while (client!.Available > 0 && !cancellationTokenSource.IsCancellationRequested)
        {
            var buffer = stream!.ReadPacket();
            var packetId = buffer.ReadVarInt();
            var packetType = Data.Protocol.GetPacketType(PacketFlow.Clientbound, GameState, packetId);

            if (bundlePackets)
            {
                bundledPackets.Enqueue((packetType, buffer));
            }
            else
            {
                await HandleIncomingPacket(packetType, buffer, GameState == GameState.Login);
            }

            if (GameState != GameState.Play)
            {
                await Task.Delay(1);
            }
        }
    }

    private async Task SendPackets()
    {
        if (!packetQueue.TryDequeue(out var task))
        {
            return;
        }

        if (task.Token is { IsCancellationRequested: true })
        {
            return;
        }

        DispatchPacket(task.Packet);

        _ = Task.Run(() => task.Task.TrySetResult());
        await HandleOutgoingPacket(task.Packet);
    }

    private void DispatchPacket(IPacket packet)
    {
        var packetId = Data.Protocol.GetPacketId(packet.Type);

        var buffer = new PacketBuffer(Data.Version.Protocol);
        buffer.WriteVarInt(packetId);
        packet.Write(buffer, Data);

        stream!.WritePacket(buffer);
    }

    private async Task HandleIncomingPacket(PacketType packetType, PacketBuffer buffer, bool block)
    {
        // Only parse packets that are awaited by an handler.
        // Possible handlers are:
        //  - MinecraftClient.On<T>()
        //  - MinecraftClient.WaitForPacket<T>()
        //  - MinecraftClient.OnPacketReceived     <-- Forces all packets to be parsed
        //  - The internal IPacketHandler

        var factory = PacketPalette.GetFactory(packetType);
        if (factory == null)
        {
            await buffer.DisposeAsync();
            return; // TODO: Maybe add an event for unknown packets
        }

        var handlers = new List<AsyncPacketHandler>();

        // Internal packet handler
        if (internalPacketHandler.HandlesIncoming(packetType))
        {
            handlers.Add(internalPacketHandler.HandleIncoming);
        }

        // Custom packet handlers
        if (packetHandlers.TryGetValue(packetType, out var customHandlers))
        {
            handlers.AddRange(customHandlers);
        }

        packetWaiters.TryGetValue(packetType, out var tsc);

        if (handlers.Count == 0 && tsc == null)
        {
            await buffer.DisposeAsync();
            return;
        }

        var size = buffer.ReadableBytes;
        try
        {
            var packet = factory(buffer, Data);
            await buffer.DisposeAsync();

            var handle = Task.Run(async () =>
            {
                tsc?.TrySetResult(packet);
                var tasks = handlers
                           .Select(task => task(packet))
                           .ToArray();

                try
                {
                    await Task.WhenAll(tasks);
                }
                catch (Exception)
                {
                    foreach (var exception in tasks.Where(x => x.Exception != null))
                    {
                        Logger.Warn($"Error in custom packet handling: {exception.Exception}");
                    }
                }
            });

            if (block)
            {
                await handle;
            }
        }
        catch (EndOfStreamException e)
        {
            Logger.Error(e, "Could not read packet {PacketType}, it was {Size} bytes.", packetType, size);
        }
    }

    private async Task HandleOutgoingPacket(IPacket packet)
    {
        await internalPacketHandler.HandleOutgoing(packet);
    }

    /// <summary>
    ///     Requests the server status and closes the connection.
    ///     Works only when <see cref="GameState" /> is <see cref="Core.Common.Protocol.GameState.Status" />.
    /// </summary>
    /// <returns></returns>
    public static async Task<ServerStatus> RequestServerStatus(
        string hostnameOrIp,
        ushort port = 25565,
        int timeout = 10000,
        IConnectionFactory? tcpFactory = null)
    {
        var latest = await MinecraftData.FromVersion(LATEST_SUPPORTED_VERSION);
        var client = new MinecraftClient(
            latest,
            Session.OfflineSession("RequestStatus"),
            hostnameOrIp,
            port,
            MinecraftApi.Instance,
            tcpFactory ?? DefaultTcpFactory.Instance,
            ClientSettings.Default);

        if (!await client.Connect(GameState.Status))
        {
            throw new MineSharpHostException("failed to connect to server");
        }

        var timeoutCancellation = new CancellationTokenSource();
        var taskCompletionSource = new TaskCompletionSource<ServerStatus>();

        client.On<StatusResponsePacket>(async packet =>
        {
            var json = packet.Response;
            var response = ServerStatus.Parse(JToken.Parse(json), client.Data);
            taskCompletionSource.TrySetResult(response);

            // the server closes the connection 
            // after sending the StatusResponsePacket
            await client.Disconnect();
            client.Dispose();
        });

        await client.SendPacket(new StatusRequestPacket(), timeoutCancellation.Token);
        await client.SendPacket(new PingRequestPacket(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()), timeoutCancellation.Token);

        timeoutCancellation.Token.Register(
            () => taskCompletionSource.TrySetCanceled(timeoutCancellation.Token));

        timeoutCancellation.CancelAfter(timeout);

        return await taskCompletionSource.Task;
    }

    /// <summary>
    ///     Requests the server status and returns a <see cref="MinecraftData" /> object based on
    ///     the version the server returned.
    /// </summary>
    /// <param name="hostname"></param>
    /// <param name="port"></param>
    /// <returns></returns>
    public static async Task<MinecraftData> AutodetectServerVersion(string hostname, ushort port)
    {
        var status = await RequestServerStatus(hostname, port);

        return await MinecraftData.FromVersion(status.Version);
    }

    private record PacketSendTask(IPacket Packet, CancellationToken? Token, TaskCompletionSource Task);
}
