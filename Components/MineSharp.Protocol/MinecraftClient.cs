using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks.Dataflow;
using ConcurrentCollections;
using MineSharp.Auth;
using MineSharp.ChatComponent;
using MineSharp.ChatComponent.Components;
using MineSharp.Core.Common.Protocol;
using MineSharp.Core.Concurrency;
using MineSharp.Core.Events;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
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
///     Connect to a Minecraft server.
/// </summary>
public sealed class MinecraftClient : IAsyncDisposable, IDisposable
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

    /// <summary>
    ///     The MinecraftData object of this client
    /// </summary>
    public readonly MinecraftData Data;

    /// <summary>
    ///     The Hostname of the Minecraft server provided in the constructor
    /// </summary>
    public readonly string Hostname;

    private readonly IPAddress ip;

    /// <summary>
    ///     Is cancelled once the client needs to stop. Usually because the connection was lost.
    /// </summary>
    // This variable exists (and is not a property) to prevent the possible problem when getting the token from the source when it's already disposed.
    public readonly CancellationToken CancellationToken;

    /// <summary>
    ///     The Port of the Minecraft server
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
    ///     Fired when the TCP connection to the server is lost.
    ///     This is called after <see cref="OnDisconnected"/> but also in cases where <see cref="OnDisconnected"/> is not called before.
    ///     
    ///     If you want to be notified when the client is no longer functional, register to <see cref="CancellationToken"/> instead.
    ///     <see cref="CancellationToken"/> is also cancelled when the connection is lost.
    /// </summary>
    public AsyncEvent<MinecraftClient> OnConnectionLost = new();

    private readonly IConnectionFactory tcpTcpFactory;

    private TcpClient? client;
    private MinecraftStream? stream;
    private Task? streamLoop;
    private int onConnectionLostFired;

    private readonly ConcurrentDictionary<PacketType, ConcurrentBag<AsyncPacketHandler>> packetHandlers;
    private readonly ConcurrentDictionary<PacketType, TaskCompletionSource<object>> packetWaiters;
    private readonly ConcurrentHashSet<AsyncPacketHandler> packetReceivers;
    private GameStatePacketHandler gameStatePacketHandler;
    private readonly BufferBlock<PacketSendTask> packetQueue;
    /// <summary>
    /// Contains the packets that are bundled together.
    /// 
    /// If this field is null, packets are not bundled.
    /// If this field is not null, packets are bundled.
    /// This way we can avoid all race conditions that would occur if we would use a boolean flag.
    /// </summary>
    private ConcurrentQueue<(PacketType Type, PacketBuffer Buffer)>? bundledPackets;

    private readonly CancellationTokenSource cancellationTokenSource;

    /// <summary>
    /// Will be completed once the client has entered the <see cref="GameState.Play"/> state.
    /// </summary>
    internal readonly TaskCompletionSource GameJoinedTcs;


    /// <summary>
    ///    The current <see cref="GameState"/> of the client.
    ///    
    ///    Internal note: This property should not be used to determine the next <see cref="GameStatePacketHandler"/> because that is not thread safe.
    /// </summary>
    public GameState GameState => gameStatePacketHandler.GameState;

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
        cancellationTokenSource = new();
        CancellationToken = cancellationTokenSource.Token;
        packetQueue = new(new DataflowBlockOptions()
        {
            CancellationToken = CancellationToken
        });
        gameStatePacketHandler = new NoStatePacketHandler(this);
        packetHandlers = new();
        packetWaiters = new();
        packetReceivers = new();
        GameJoinedTcs = new();
        bundledPackets = null;
        tcpTcpFactory = tcpFactory;
        ip = IpHelper.ResolveHostname(hostnameOrIp, ref port);

        Api = api;
        Session = session;
        Port = port;
        Hostname = hostnameOrIp;
        Settings = settings;
    }

    /// <summary>
    ///     Must only be called after the <see cref="packetQueue"/> was completed (this happens when <see cref="CancellationToken"/> was cancelled).
    ///     Otherwise race conditions can occur where there are still uncancelled tasks in the queue.
    /// </summary>
    private void CancelAllSendPendingPacketTasks()
    {
        // we need to cancel all tasks in the queue otherwise we might get a deadlock
        // when some task is waiting for the packet task
        if (packetQueue.TryReceiveAll(out var queuedPackets))
        {
            foreach (var task in queuedPackets)
            {
                task.Task.TrySetCanceled(CancellationToken);
            }
        }
    }

    private async Task DisposeInternal(bool calledFromStreamLoop)
    {
        cancellationTokenSource.Cancel();
        // wait for the packetQueue to complete
        await packetQueue.Completion;
        CancelAllSendPendingPacketTasks();

        // prevent waiting on ourselves (when called from the streamLoop task)
        if (!calledFromStreamLoop)
        {
            await (streamLoop ?? Task.CompletedTask);
        }

        client?.Dispose();
        stream?.Close();

        if (Interlocked.Exchange(ref onConnectionLostFired, 1) == 0)
        {
            await OnConnectionLost.Dispatch(this);
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeInternal(false);
    }


    /// <inheritdoc />
    public void Dispose()
    {
        DisposeAsync().AsTask().Wait();
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

            streamLoop = Task.Run(StreamLoop);
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
    public async Task SendPacket(IPacket packet, CancellationToken cancellation = default)
    {
        var sendingTask = new PacketSendTask(packet, cancellation, new());
        try
        {
            if (!await packetQueue.SendAsync(sendingTask, cancellation))
            {
                // if the packetQueue is completed we can not send any more packets
                // so we need to cancel the task here
                // this must have happened because the CancellationToken was cancelled
                Logger.Warn("Packet {PacketType} could not be added send queue. Queue closed.", packet.Type);
                sendingTask.Task.TrySetCanceled(CancellationToken);
            }
            else
            {
                Logger.Trace("Packet {PacketType} was added to send queue", packet.Type);
            }
        }
        catch (OperationCanceledException e)
        {
            Logger.Warn("Packet {PacketType} could not be added send queue. Sending Packet CancellationToken was cancelled.", packet.Type);
            sendingTask.Task.TrySetCanceled(e.CancellationToken);
            throw;
        }

        await sendingTask.Task.Task;
    }

    private Task? disconnectTask;

    /// <summary>
    ///     Disconnects the client from the server.
    /// </summary>
    /// <param name="reason">The reason the client disconnected. Only used for the <see cref="OnDisconnected" /> event.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public EnsureOnlyRunOnceAsyncResult Disconnect(Chat? reason = null)
    {
        return ConcurrencyHelper.EnsureOnlyRunOnceAsync(() => DisconnectInternal(reason), ref disconnectTask);
    }

    private async Task DisconnectInternal(Chat? reason = null)
    {
        reason ??= new TranslatableComponent("disconnect.quitting");

        var message = reason.GetMessage(Data);
        Logger.Info($"Disconnecting: {message}");

        GameJoinedTcs.TrySetException(new DisconnectedException("Client has been disconnected", message));

        if (client is null || !client.Connected)
        {
            Logger.Warn("Disconnect() was called but client is not connected");
        }

        await cancellationTokenSource.CancelAsync();
        await (streamLoop ?? Task.CompletedTask);

        client?.Close();
        await OnDisconnected.Dispatch(this, reason);
        await OnConnectionLost.Dispatch(this);
    }

    /// <summary>
    ///     Registers an <paramref name="handler" /> that will be called whenever an packet of type <typeparamref name="T" />
    ///     is received
    /// </summary>
    /// <param name="handler">A delegate that will be called when a packet of type T is received</param>
    /// <typeparam name="T">The type of the packet</typeparam>
    public void On<T>(AsyncPacketHandler<T> handler) where T : IPacket
    {
        var key = T.StaticType;

        packetHandlers.GetOrAdd(key, _ => new ConcurrentBag<AsyncPacketHandler>())
                      .Add(p => handler((T)p));
    }

    /// <summary>
    ///     Waits until a packet of the specified type is received.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>A task that completes once the packet is received</returns>
    public Task<T> WaitForPacket<T>() where T : IPacket
    {
        var packetType = T.StaticType;
        var tcs = packetWaiters.GetOrAdd(packetType, _ => new TaskCompletionSource<object>());
        return tcs.Task.ContinueWith(prev => (T)prev.Result);
    }

    public sealed class OnPacketReceivedRegistration : IDisposable
    {
        private readonly MinecraftClient client;
        private readonly AsyncPacketHandler handler;
        public bool Disposed { get; private set; }

        internal OnPacketReceivedRegistration(MinecraftClient client, AsyncPacketHandler handler)
        {
            this.client = client;
            this.handler = handler;
        }

        public void Dispose()
        {
            if (Disposed)
            {
                return;
            }
            client.packetReceivers.TryRemove(handler);
            Disposed = true;
        }
    }

    /// <summary>
    /// Registers a handler that will be called whenever a packet is received.
    /// CAUTION: This will parse all packets, even if they are not awaited by any other handler.
    /// This can lead to performance issues if the server sends a lot of packets.
    /// Use this for debugging purposes only.
    /// </summary>
    /// <param name="handler">A delegate that will be called when a packet is received.</param>
    public OnPacketReceivedRegistration? OnPacketReceived(AsyncPacketHandler handler)
    {
        var added = packetReceivers.Add(handler);
        return added ? new(this, handler) : null;
    }

    /// <summary>
    ///     Waits until the client jumps into the <see cref="GameState.Play" /> state.
    /// </summary>
    /// <returns></returns>
    public Task WaitForGame()
    {
        return GameJoinedTcs.Task;
    }

    internal Task SendClientInformationPacket()
    {
        return SendPacket(new ClientInformationPacket(
            Settings.Locale,
            Settings.ViewDistance,
            Settings.ChatMode,
            Settings.ColoredChat,
            Settings.DisplayedSkinParts,
            Settings.MainHand,
            Settings.EnableTextFiltering,
            Settings.AllowServerListings));
    }

    internal async Task ChangeGameState(GameState next)
    {
        GameStatePacketHandler newGameStatePacketHandler = next switch
        {
            GameState.Handshaking => new HandshakePacketHandler(this),
            GameState.Login => new LoginPacketHandler(this, Data),
            GameState.Status => new StatusPacketHandler(this),
            GameState.Configuration => new ConfigurationPacketHandler(this, Data),
            GameState.Play => new PlayPacketHandler(this, Data),
            _ => throw new UnreachableException()
        };
        gameStatePacketHandler = newGameStatePacketHandler;
        await newGameStatePacketHandler.StateEntered();
    }

    internal void EnableEncryption(byte[] key)
    {
        stream!.EnableEncryption(key);
    }

    internal void SetCompression(int threshold)
    {
        stream!.SetCompression(threshold);
    }

    internal async Task HandleBundleDelimiter()
    {
        var bundledPackets = Interlocked.Exchange(ref this.bundledPackets, null);
        if (bundledPackets != null)
        {
            Logger.Debug("Processing bundled packets");
            var tasks = bundledPackets.Select(
                                           p => HandleIncomingPacket(p.Type, p.Buffer))
                                      .ToArray();

            // no clearing required the queue will no longer be used and will get GCed
            //bundledPackets.Clear();

            await Task.WhenAll(tasks);

            foreach (var faultedTask in tasks.Where(task => task.Status == TaskStatus.Faulted))
            {
                Logger.Error(faultedTask.Exception, "Error handling bundled packet.");
            }
        }
        else
        {
            if (Interlocked.CompareExchange(ref this.bundledPackets, new(), null) == null)
            {
                Logger.Debug("Bundling packets!");
            }
            else
            {
                Logger.Warn("Bundling could not be enabled because it was already enabled. This is a race condition.");
            }
        }
    }

    private async Task StreamLoop()
    {
        try
        {
            // run both tasks in parallel
            var receiveTask = Task.Factory.StartNew(ReceivePackets, TaskCreationOptions.LongRunning);
            var sendTask = Task.Factory.StartNew(SendPackets, TaskCreationOptions.LongRunning);

            // extract the exception from the task that finished first
            await await Task.WhenAny(receiveTask, sendTask);
            // DisposeInternal in the catch block will then stop the other task
        }
        catch (Exception e)
        {
            // EndOfStreamException is expected when the connection is closed
            if (e is not EndOfStreamException)
            {
                Logger.Error(e, "Encountered exception in outer stream loop. Connection will be terminated.");
            }
            await DisposeInternal(true);
        }
    }

    private async Task ReceivePackets()
    {
        while (true)
        {
            CancellationToken.ThrowIfCancellationRequested();

            var buffer = stream!.ReadPacket();

            var packetId = buffer.ReadVarInt();
            var gameState = gameStatePacketHandler.GameState;
            var packetType = Data.Protocol.GetPacketType(PacketFlow.Clientbound, gameState, packetId);

            Logger.Trace("Received packet {PacketType}. GameState = {GameState}, PacketId = {PacketId}", packetType, gameState, packetId);

            if (gameState != GameState.Play)
            {
                await HandleIncomingPacket(packetType, buffer);
            }
            else
            {
                var bundledPackets = this.bundledPackets;
                if (bundledPackets != null)
                {
                    bundledPackets.Enqueue((packetType, buffer));
                }
                else
                {
                    // handle the packet in a new task to prevent blocking the stream loop
                    _ = Task.Run(() => HandleIncomingPacket(packetType, buffer));
                }
            }
        }
    }

    private async Task SendPackets()
    {
        await foreach (var task in packetQueue.ReceiveAllAsync())
        {
            if (task.Token.IsCancellationRequested)
            {
                task.Task.TrySetCanceled();
                continue;
            }

            try
            {
                DispatchPacket(task.Packet);
                // TrySetResult must be run from a different task to prevent blocking the stream loop
                // because the task continuation will be executed inline and might block or cause a deadlock
                _ = Task.Run(task.Task.TrySetResult);
            }
            catch (SocketException e)
            {
                Logger.Error(e, "Encountered exception while dispatching packet {PacketType}", task.Packet.Type);
                task.Task.TrySetException(e);
                // break the loop to prevent further packets from being sent
                // because the connection is probably dead
                throw;
            }
            catch (Exception e)
            {
                task.Task.TrySetException(e);
            }
        }
    }

    private void DispatchPacket(IPacket packet)
    {
        var packetId = Data.Protocol.GetPacketId(packet.Type);

        var buffer = new PacketBuffer(Data.Version.Protocol);
        buffer.WriteVarInt(packetId);
        packet.Write(buffer, Data);

        try
        {
            Logger.Trace("Sending packet {PacketType}", packet.Type);
            stream!.WritePacket(buffer);
        }
        catch (SocketException e)
        {
            Logger.Error(e, "Encountered exception while dispatching packet {PacketType}", packet.Type);
            throw;
        }
    }

    // TODO: object is bad but IPacket is not allowed as generic type
    private async Task<object?> ParsePacket(PacketPalette.PacketFactory packetFactory, PacketType packetType, PacketBuffer buffer)
    {
        var size = buffer.ReadableBytes;
        try
        {
            var packet = packetFactory(buffer, Data);

            var unreadBytes = buffer.ReadableBytes;
            if (unreadBytes != 0)
            {
                Logger.Warn("After reading the packet {PacketType}, the buffer still contains {unreadBytes}/{Size} bytes.", packetType, unreadBytes, size);
            }

            return packet;
        }
        catch (Exception e)
        {
            Logger.Error(e, "Could not read packet {PacketType}, it was {Size} bytes.", packetType, size);
        }
        finally
        {
            await buffer.DisposeAsync();
        }
        return null;
    }

    private async Task HandleIncomingPacket(PacketType packetType, PacketBuffer buffer)
    {
        // Only parse packets that are awaited by an handler.
        // Possible handlers are:
        //  - MinecraftClient.On<T>()
        //  - MinecraftClient.WaitForPacket<T>()
        //  - MinecraftClient.OnPacketReceived     <-- Forces all packets to be parsed
        //  - The internal IPacketHandler

        Logger.Trace("Handling packet {PacketType}", packetType);
        var factory = PacketPalette.GetFactory(packetType);
        if (factory == null)
        {
            await buffer.DisposeAsync();
            return; // TODO: Maybe add an event for unknown packets
        }

        var handlers = new List<AsyncPacketHandler>();

        // GameState packet handler
        if (gameStatePacketHandler.HandlesIncoming(packetType))
        {
            handlers.Add(gameStatePacketHandler.HandleIncoming);
        }

        // Custom packet handlers
        if (packetHandlers.TryGetValue(packetType, out var customHandlers))
        {
            handlers.AddRange(customHandlers);
        }

        packetWaiters.TryGetValue(packetType, out var tcs);

        if (handlers.Count == 0 && tcs == null && packetReceivers.IsEmpty)
        {
            await buffer.DisposeAsync();
            return;
        }

        var packet = (IPacket?) await ParsePacket(factory, packetType, buffer);

        if (packet == null)
        {
            // The packet could not be parsed
            return;
        }

        tcs?.TrySetResult(packet);

        var packetHandlersTasks = new List<Task>();
        try
        {
            // Run all handlers in parallel:
            foreach (var packetReceiver in packetReceivers)
            {
                // The synchronous part of the handlers might throw an exception
                // So we also do this in a try-catch block
                try
                {
                    packetHandlersTasks.Add(packetReceiver(packet));
                }
                catch (Exception e)
                {
                    Logger.Warn(e, "An packet receiver threw an exception when receiving a packet of type {PacketType}.", packetType);
                }
            }

            foreach (var handler in handlers)
            {
                // The synchronous part of the handlers might throw an exception
                // So we also do this in a try-catch block
                try
                {
                    packetHandlersTasks.Add(handler(packet));
                }
                catch (Exception e)
                {
                    Logger.Warn(e, "An packet handler for packet of type {PacketType} threw an exception.", packetType);
                }
            }

            await Task.WhenAll(packetHandlersTasks);
        }
        catch (Exception)
        {
            foreach (var faultedTask in packetHandlersTasks.Where(task => task.Status == TaskStatus.Faulted))
            {
                Logger.Warn(faultedTask.Exception, "An packet handler for packet of type {PacketType} threw an exception.", packetType);
            }
        }
    }

    /// <summary>
    ///     Requests the server status and closes the connection.
    ///     Works only when <see cref="GameState"/> is <see cref="Core.Common.Protocol.GameState.Status" />.
    /// </summary>
    /// <param name="hostnameOrIp">The hostname or IP of the server.</param>
    /// <param name="port">The port of the server.</param>
    /// <param name="responseTimeout">The time in milliseconds to wait for a response.</param>
    /// <param name="tcpFactory">The factory to create the TCP connection.</param>
    /// <returns></returns>
    public static async Task<ServerStatus> RequestServerStatus(
        string hostnameOrIp,
        ushort port = 25565,
        int responseTimeout = 10000,
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

        var responseTimeoutCts = new CancellationTokenSource();
        var responseTimeoutCancellationToken = responseTimeoutCts.Token;
        var taskCompletionSource = new TaskCompletionSource<ServerStatus>();

        client.On<StatusResponsePacket>(async packet =>
        {
            var json = packet.Response;
            var response = ServerStatus.FromJToken(JToken.Parse(json), client.Data);
            taskCompletionSource.TrySetResult(response);

            // the server closes the connection 
            // after sending the StatusResponsePacket
            // so just dispose the client (no point in disconnecting)
            await client.DisposeAsync();
        });

        await client.SendPacket(new StatusRequestPacket(), responseTimeoutCancellationToken);
        await client.SendPacket(new PingRequestPacket(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()), responseTimeoutCancellationToken);

        responseTimeoutCancellationToken.Register(
            () =>
            {
                taskCompletionSource.TrySetCanceled(responseTimeoutCancellationToken);
                responseTimeoutCts.Dispose();
            });

        responseTimeoutCts.CancelAfter(responseTimeout);

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

    private record PacketSendTask(IPacket Packet, CancellationToken Token, TaskCompletionSource Task);
}
