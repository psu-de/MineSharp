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
using MineSharp.Protocol.Packets.Serverbound.Status;
using MineSharp.Protocol.Registrations;
using Newtonsoft.Json.Linq;
using NLog;
using ConfigurationClientInformationPacket = MineSharp.Protocol.Packets.Serverbound.Configuration.ClientInformationPacket;
using PlayClientInformationPacket = MineSharp.Protocol.Packets.Serverbound.Play.ClientInformationPacket;

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

    private readonly ConcurrentDictionary<PacketType, ConcurrentHashSet<AsyncPacketHandler>> packetHandlers;
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
        GameJoinedTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
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

        Logger.Debug($"Connecting to {ip}:{Port} with PVN={Data.Version.Protocol}");

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
        var sendingTask = new PacketSendTask(packet, cancellation, new(TaskCreationOptions.RunContinuationsAsynchronously));
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
    /// Represents a registration for a packet handler that will be called whenever a packet of type <typeparamref name="T" /> is received.
    /// This registration can be used to unregister the handler.
    /// </summary>
    /// <typeparam name="T">The type of the packet.</typeparam>
    public sealed class OnPacketRegistration<T> : AbstractPacketReceiveRegistration
         where T : IPacket
    {
        internal OnPacketRegistration(MinecraftClient client, AsyncPacketHandler handler)
            : base(client, handler)
        {
        }

        /// <inheritdoc/>
        protected override void Unregister()
        {
            var key = T.StaticType;
            if (Client.packetHandlers.TryGetValue(key, out var handlers))
            {
                handlers.TryRemove(Handler);
            }
        }
    }

    /// <summary>
    ///     Registers an <paramref name="handler" /> that will be called whenever an packet of type <typeparamref name="T" />
    ///     is received
    /// </summary>
    /// <param name="handler">A delegate that will be called when a packet of type T is received</param>
    /// <typeparam name="T">The type of the packet</typeparam>
    /// <returns>A registration object that can be used to unregister the handler.</returns>
    public OnPacketRegistration<T>? On<T>(AsyncPacketHandler<T> handler) where T : IPacket
    {
        var key = T.StaticType;
        AsyncPacketHandler rawHandler = packet => handler((T)packet);
        var added = packetHandlers.GetOrAdd(key, _ => new ConcurrentHashSet<AsyncPacketHandler>())
                      .Add(rawHandler);
        return added ? new(this, rawHandler) : null;
    }

    /// <summary>
    /// Waits until a packet of the specified type is received and matches the given condition.
    /// </summary>
    /// <typeparam name="TPacket">The type of the packet.</typeparam>
    /// <param name="condition">A function that evaluates the packet and returns true if the condition is met.</param>
    /// <param name="cancellationToken">A token to cancel the wait for the matching packet.</param>
    /// <returns>A task that completes once a packet matching the condition is received.</returns>
    public Task<TPacket> WaitForPacketWhere<TPacket>(Func<TPacket, Task<bool>> condition, CancellationToken cancellationToken = default)
         where TPacket : IPacket
    {
        // linked token is required to cancel the task when the client is disconnected
        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, CancellationToken);
        var token = cts.Token;
        var tcs = new TaskCompletionSource<TPacket>(TaskCreationOptions.RunContinuationsAsynchronously);
        async Task PacketHandler(TPacket packet)
        {
            try
            {
                if (tcs.Task.IsCompleted)
                {
                    return;
                }
                if (await condition(packet).WaitAsync(token))
                {
                    tcs.TrySetResult(packet);
                }
            }
            catch (OperationCanceledException e)
            {
                tcs.TrySetCanceled(e.CancellationToken);
            }
            catch (Exception e)
            {
                tcs.TrySetException(e);
            }
        }
        var packetRegistration = On<TPacket>(PacketHandler);
        if (packetRegistration == null)
        {
            // TODO: Can this occur?
            cts.Dispose();
            throw new InvalidOperationException("Could not register packet handler");
        }
        // this registration is required because otherwise the task will only get cancelled when the next packet of that ype is received
        var cancellationRegistration = token.Register(() =>
        {
            // cancelling the tcs will later dispose the other stuff
            tcs.TrySetCanceled(token);
        });
        tcs.Task.ContinueWith(_ =>
        {
            cancellationRegistration.Dispose();
            packetRegistration.Dispose();
            cts.Dispose();
        }, TaskContinuationOptions.ExecuteSynchronously);
        return tcs.Task;
    }

    /// <inheritdoc cref="WaitForPacketWhere{TPacket}(Func{TPacket, Task{bool}}, CancellationToken)"/>
    public Task<TPacket> WaitForPacketWhere<TPacket>(Func<TPacket, bool> condition, CancellationToken cancellationToken = default)
        where TPacket : IPacket
    {
        return WaitForPacketWhere<TPacket>(packet => Task.FromResult(condition(packet)), cancellationToken);
    }

    /// <summary>
    ///     Waits until a packet of the specified type is received.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>A task that completes once the packet is received</returns>
    public Task<T> WaitForPacket<T>() where T : IPacket
    {
        var packetType = T.StaticType;
        var tcs = packetWaiters.GetOrAdd(packetType, _ => new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously));
        return tcs.Task.ContinueWith(prev => (T)prev.Result);
    }

    /// <summary>
    /// Represents a registration for a packet handler that will be called whenever any packet is received.
    /// This registration can be used to unregister the handler.
    /// </summary>
    public sealed class OnPacketReceivedRegistration : AbstractPacketReceiveRegistration
    {
        internal OnPacketReceivedRegistration(MinecraftClient client, AsyncPacketHandler handler)
            : base(client, handler)
        {
        }

        /// <inheritdoc/>
        protected override void Unregister()
        {
            Client.packetReceivers.TryRemove(Handler);
        }
    }

    /// <summary>
    /// Registers a handler that will be called whenever a packet is received.
    /// CAUTION: This will parse all packets, even if they are not awaited by any other handler.
    /// This can lead to performance issues if the server sends a lot of packets.
    /// Use this for debugging purposes only.
    /// </summary>
    /// <param name="handler">A delegate that will be called when a packet is received.</param>
    /// <returns>A registration object that can be used to unregister the handler.</returns>
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

    internal Task SendClientInformationPacket(GameState gameState)
    {
        IPacket packet = gameState switch
        {
            GameState.Configuration => new ConfigurationClientInformationPacket(
                Settings.Locale,
                Settings.ViewDistance,
                Settings.ChatMode,
                Settings.ColoredChat,
                Settings.DisplayedSkinParts,
                Settings.MainHand,
                Settings.EnableTextFiltering,
                Settings.AllowServerListings),
            GameState.Play => new PlayClientInformationPacket(
                Settings.Locale,
                Settings.ViewDistance,
                Settings.ChatMode,
                Settings.ColoredChat,
                Settings.DisplayedSkinParts,
                Settings.MainHand,
                Settings.EnableTextFiltering,
                Settings.AllowServerListings),
            _ => throw new NotImplementedException(),
        };
        return SendPacket(packet);
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

    internal void HandleBundleDelimiter()
    {
        var bundledPackets = Interlocked.Exchange(ref this.bundledPackets, null);
        if (bundledPackets != null)
        {
            _ = Task.Run(() => ProcessBundledPackets(bundledPackets), CancellationToken);
        }
        else
        {
            if (Interlocked.CompareExchange(ref this.bundledPackets, new(), null) != null)
            {
                Logger.Warn("Bundling could not be enabled because it was already enabled. This is a race condition.");
            }
        }
    }

    private async Task ProcessBundledPackets(ConcurrentQueue<(PacketType, PacketBuffer)> packets)
    {
        Logger.Trace($"Processing {packets.Count} bundled packets");
        try
        {
            // wiki.vg: the client is guaranteed to process every packet in the bundle on the same tick
            // we don't guarantee that.
            // TODO: process bundled packets within a single tick
            var tasks = packets.Select(
                                    p => HandleIncomingPacket(p.Item1, p.Item2))
                               .ToArray();

            // no clearing required the queue will no longer be used and will get GCed
            // bundledPackets.Clear();

            await Task.WhenAll(tasks);

            foreach (var faultedTask in tasks.Where(task => task.Status == TaskStatus.Faulted))
            {
                Logger.Error(faultedTask.Exception, "Error handling bundled packet.");
            }
        }
        catch (Exception e)
        {
            Logger.Error(e, "Error handling bundled packets.");
        }
    }

    private async Task StreamLoop()
    {
        try
        {
            // run both tasks in parallel
            // because the task factory does not unwrap the tasks (like Task.Run) we need to do it manually
            var receiveTask = Task.Factory.StartNew(ReceivePackets, TaskCreationOptions.LongRunning).Unwrap();
            var sendTask = Task.Factory.StartNew(SendPackets, TaskCreationOptions.LongRunning).Unwrap();

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
        try
        {
            while (true)
            {
                CancellationToken.ThrowIfCancellationRequested();

                var buffer = stream!.ReadPacket();

                var packetId = buffer.ReadVarInt();
                var gameState = gameStatePacketHandler.GameState;
                var packetType = Data.Protocol.GetPacketType(PacketFlow.Clientbound, gameState, packetId);

                Logger.Trace("Received packet {PacketType}. GameState = {GameState}, PacketId = {PacketId}", packetType, gameState, packetId);

                // handle BundleDelimiter packet here, because there is a race condition where some
                // packets may be read before HandleBundleDelimiter is invoked through a handler
                if (packetType == PacketType.CB_Play_BundleDelimiter)
                {
                    HandleBundleDelimiter();
                    continue;
                }

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
        catch (Exception e)
        {
            Logger.Debug(e, "ReceivePackets loop ended with exception.");
            throw;
        }
        // can never exit without exception because infinite loop without break
    }

    private async Task SendPackets()
    {
        try
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
                    task.Task.TrySetResult();
                }
                catch (OperationCanceledException e)
                {
                    task.Task.TrySetCanceled(e.CancellationToken);
                    // we should stop. So we do by rethrowing the exception
                    throw;
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Encountered exception while dispatching packet {PacketType}", task.Packet.Type);
                    task.Task.TrySetException(e);
                    if (e is SocketException)
                    {
                        // break the loop to prevent further packets from being sent
                        // because the connection is probably dead
                        throw;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Logger.Debug(e, "SendPackets loop ended with exception.");
            throw;
        }
        // can never exit without exception because infinite loop without break (because we never complete the BufferBlock we only cancel it)
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

        var packet = (IPacket?)await ParsePacket(factory, packetType, buffer);

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

        using var responseTimeoutCts = new CancellationTokenSource();
        var responseTimeoutCancellationToken = responseTimeoutCts.Token;

        var statusResponsePacketTask = client.WaitForPacket<StatusResponsePacket>();

        await client.SendPacket(new StatusRequestPacket(), responseTimeoutCancellationToken);
        await client.SendPacket(new PingRequestPacket(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()), responseTimeoutCancellationToken);

        responseTimeoutCts.CancelAfter(responseTimeout);

        var statusResponsePacket = await statusResponsePacketTask.WaitAsync(responseTimeoutCancellationToken);
        var json = statusResponsePacket.Response;
        var response = ServerStatus.FromJToken(JToken.Parse(json), client.Data);

        // the server closes the connection 
        // after sending the StatusResponsePacket and PingResponsePacket
        // so just dispose the client (no point in disconnecting)
        try
        {
            await client.DisposeAsync();
        }
        catch (Exception)
        {
            // ignore all errors
            // in most cases the exception is an OperationCanceledException because the connection was terminated
        }

        return response;
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
