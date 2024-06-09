using System.Collections.Concurrent;
using MineSharp.Auth;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Protocol;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Exceptions;
using MineSharp.Protocol.Packets;
using MineSharp.Protocol.Packets.Clientbound.Status;
using MineSharp.Protocol.Packets.Handlers;
using MineSharp.Protocol.Packets.Serverbound.Status;
using NLog;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using MineSharp.Protocol.Packets.Serverbound.Configuration;
using Newtonsoft.Json.Linq;

namespace MineSharp.Protocol;

/// <summary>
/// A Minecraft client.
/// Connect to a minecraft server.
/// </summary>
public sealed class MinecraftClient : IDisposable
{
    /// <summary>
    /// The latest version supported
    /// </summary>
    public const string LATEST_SUPPORTED_VERSION = "1.20.4";

    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Delegate for handling packets async
    /// </summary>
    public delegate Task AsyncPacketHandler(IPacket packet);

    /// <summary>
    /// Delegate for handling a specific packet async
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public delegate Task AsyncPacketHandler<in T>(T packet) where T : IPacket;

    internal readonly MinecraftApi? Api;

    private readonly CancellationTokenSource _cancellation;
    private readonly ITcpClientFactory?      _tcpTcpFactory;

    private readonly Queue<(PacketType, PacketBuffer)>                     _bundledPackets;
    private readonly ConcurrentQueue<PacketSendTask>                       _packetQueue;
    private readonly IDictionary<PacketType, IList<AsyncPacketHandler>>    _packetHandlers;
    private readonly IDictionary<PacketType, TaskCompletionSource<object>> _packetWaiters;
    private readonly TaskCompletionSource                                  _gameJoinedTsc;
    private readonly bool                                                  _useAnonymousNbt;

    private GameState        gameState;
    private bool             _bundlePackets;
    private TcpClient?       _client;
    private IPacketHandler   _internalPacketHandler;
    private MinecraftStream? _stream;
    private Task?            _streamLoop;
    private IPAddress        ip;

    /// <summary>
    /// Fires whenever the client received a known Packet
    /// </summary>
    public event Events.ClientPacketEvent? OnPacketReceived;

    /// <summary>
    /// Fires whenever the client has sent a packet
    /// </summary>
    public event Events.ClientPacketEvent? OnPacketSent;

    /// <summary>
    /// Fires when the client disconnected from the server
    /// </summary>
    public event Events.ClientStringEvent? OnDisconnected;


    /// <summary>
    /// The MinecraftData object of this client
    /// </summary>
    public readonly MinecraftData Data;

    /// <summary>
    /// The Session object for this client
    /// </summary>
    public readonly Session Session;

    /// <summary>
    /// The Hostname of the minecraft server provided in the constructor
    /// </summary>
    public readonly string Hostname;

    /// <summary>
    /// The Port of the minecraft server
    /// </summary>
    public readonly ushort Port;

    /// <summary>
    /// The clients settings
    /// </summary>
    public readonly ClientSettings Settings;

    /// <summary>
    /// Create a new MinecraftClient
    /// </summary>
    public MinecraftClient(
        MinecraftData      data,
        Session            session,
        string             hostnameOrIp,
        ushort             port       = 25565,
        MinecraftApi?      api        = null,
        ITcpClientFactory? tcpFactory = null,
        ClientSettings?    settings   = null)
    {
        this.Data                   = data;
        this._packetQueue           = new ConcurrentQueue<PacketSendTask>();
        this._cancellation          = new CancellationTokenSource();
        this._internalPacketHandler = new HandshakePacketHandler(this);
        this._packetHandlers        = new Dictionary<PacketType, IList<AsyncPacketHandler>>();
        this._packetWaiters         = new Dictionary<PacketType, TaskCompletionSource<object>>();
        this._gameJoinedTsc         = new TaskCompletionSource();
        this._bundledPackets        = new Queue<(PacketType, PacketBuffer)>();
        this._useAnonymousNbt       = this.Data.Version.Protocol >= ProtocolVersion.V_1_20_2;
        this._tcpTcpFactory         = tcpFactory;
        this.ip                     = IPHelper.ResolveHostname(hostnameOrIp, ref port);
        
        if (session.OnlineSession)
            api ??= new MinecraftApi();

        this.Api       = api;
        this.Session   = session;
        this.Port      = port;
        this.Hostname  = hostnameOrIp;
        this.gameState = GameState.Handshaking;
        this.Settings  = settings ?? ClientSettings.Default;
    }

    /// <summary>
    /// Connects to the minecraft sever.
    /// </summary>
    /// <param name="nextState">The state to transition to during the handshaking process.</param>
    /// <returns>A task that resolves once connected. Results in true when connected successfully, false otherwise.</returns>
    public async Task<bool> Connect(GameState nextState)
    {
        if (this._client is not null && this._client.Connected)
        {
            Logger.Warn($"Client is already connected!");
            return false;
        }

        Logger.Debug($"Connecting to {this.ip}:{this.Port}.");

        try
        {
            if (this._tcpTcpFactory == null)
            {
                this._client = new TcpClient();
                await this._client.ConnectAsync(this.ip, this.Port, this._cancellation.Token);
            }
            else
            {
                this._client = this._tcpTcpFactory.CreateOpenConnection(this.ip.ToString(), this.Port);
            }

            this._stream = new MinecraftStream(this._client.GetStream(), this._useAnonymousNbt);

            this.StreamLoop();
            Logger.Info("Connected, starting handshake...");
            await HandshakeProtocol.PerformHandshake(this, nextState, this.Data);
        }
        catch (SocketException ex)
        {
            Logger.Error(ex, "Error while connecting");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Sends a Packet to the Minecraft Server
    /// </summary>
    /// <param name="packet">The packet to send.</param>
    /// <param name="cancellation">Optional cancellation token.</param>
    /// <returns>A task that resolves once the packet was actually sent.</returns>
    public Task SendPacket(IPacket packet, CancellationToken? cancellation = null)
    {
        var sendingTask = new PacketSendTask(packet, cancellation, new TaskCompletionSource());
        this._packetQueue.Enqueue(sendingTask);

        return sendingTask.Task.Task;
    }

    /// <summary>
    ///  Disconnects the client from the server.
    /// </summary>
    /// <param name="reason">The reason the client disconnected. Only used for the <see cref="OnDisconnected"/> event.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task Disconnect(string reason = "disconnect.quitting")
    {
        Logger.Info($"Disconnecting: {reason}");
        
        if (!_gameJoinedTsc.Task.IsCompleted)
            _gameJoinedTsc.SetException(new DisconnectedException($"Client has been disconnected", reason));
        
        if (this._client is null)
            throw new InvalidOperationException("MinecraftClient is not connected.");

        if (!this._client.Connected)
        {
            throw new InvalidOperationException("Client is not connected.");
        }

        this._cancellation.Cancel();
        await this._streamLoop!;

        this._client.Close();
        this.OnDisconnected?.Invoke(this, reason);
    }

    /// <summary>
    /// Registers an <paramref name="handler"/> that will be called whenever an packet of type <typeparamref name="T"/> is received
    /// </summary>
    /// <param name="handler">A delegate that will be called when a packet of type T is received</param>
    /// <typeparam name="T">The type of the packet</typeparam>
    public void On<T>(AsyncPacketHandler<T> handler) where T : IPacket
    {
        var key = PacketPalette.GetPacketType<T>();

        if (!this._packetHandlers.TryGetValue(key, out var handlers))
        {
            handlers = new List<AsyncPacketHandler>();
            this._packetHandlers.Add(key, handlers);
        }

        handlers.Add(p => handler((T)p));
    }

    /// <summary>
    /// Waits until a packet of the specified type is received.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>A task that completes once the packet is received</returns>
    public Task<T> WaitForPacket<T>() where T : IPacket
    {
        var packetType = PacketPalette.GetPacketType<T>();
        if (!this._packetWaiters.TryGetValue(packetType, out var task))
        {
            var tsc = new TaskCompletionSource<object>();
            this._packetWaiters.Add(packetType, tsc);

            return tsc.Task.ContinueWith(prev => (T)prev.Result);
        }

        return task.Task.ContinueWith(prev => (T)prev.Result);
    }

    /// <summary>
    /// Waits until the client jumps into the Play <see cref="gameState"/>
    /// </summary>
    /// <returns></returns>
    public Task WaitForGame()
        => this._gameJoinedTsc.Task;

    internal void UpdateGameState(GameState next)
    {
        this.gameState = next;

        this._internalPacketHandler = next switch
        {
            GameState.Handshaking   => new HandshakePacketHandler(this),
            GameState.Login         => new LoginPacketHandler(this, this.Data),
            GameState.Status        => new StatusPacketHandler(this),
            GameState.Configuration => new ConfigurationPacketHandler(this, this.Data),
            GameState.Play          => new PlayPacketHandler(this, this.Data),
            _                       => throw new UnreachableException()
        };

        if (next == GameState.Play)
            this._gameJoinedTsc.TrySetResult();

        if (next == GameState.Configuration)
        {
            this.SendPacket(new ClientInformationPacket(
                this.Settings.Locale,
                this.Settings.ViewDistance,
                (int)this.Settings.ChatMode,
                this.Settings.ColoredChat,
                this.Settings.DisplayedSkinParts,
                (int)this.Settings.MainHand,
                this.Settings.EnableTextFiltering,
                this.Settings.AllowServerListings));
        }
    }

    internal void EnableEncryption(byte[] key)
        => this._stream!.EnableEncryption(key);

    internal void SetCompression(int threshold)
        => this._stream!.SetCompression(threshold);

    internal void HandleBundleDelimiter()
    {
        this._bundlePackets = !this._bundlePackets;
        if (!this._bundlePackets)
        {
            Logger.Debug("Processing bundled packets");
            var tasks = this._bundledPackets.Select(
                                 p => this.HandleIncomingPacket(p.Item1, p.Item2, false))
                            .ToArray();

            Task.WaitAll(tasks);

            var errors = tasks.Where(x => x.Exception != null);
            foreach (var error in errors)
            {
                Logger.Error("Error handling bundled packet: {e}", error);
            }

            this._bundledPackets.Clear();
        }
        else
        {
            Logger.Debug("Bundling packets!");
        }
    }

    private void StreamLoop()
    {
        this._streamLoop = Task.Run(async () =>
        {
            while (!this._cancellation.Token.IsCancellationRequested)
            {
                try
                {
                    await this.ReceivePackets();
                    await this.SendPackets();

                    await Task.Delay(1);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Encountered error in stream loop");
                }
            }
        }, this._cancellation.Token);
    }

    private async Task ReceivePackets()
    {
        while (this._client!.Available > 0 && !this._cancellation.IsCancellationRequested)
        {
            var buffer     = this._stream!.ReadPacket();
            var packetId   = buffer.ReadVarInt();
            var packetType = this.Data.Protocol.GetPacketType(PacketFlow.Clientbound, this.gameState, packetId);

            if (this._bundlePackets)
                this._bundledPackets.Enqueue((packetType, buffer));
            else await this.HandleIncomingPacket(packetType, buffer, this.gameState == GameState.Login);

            if (this.gameState != GameState.Play)
            {
                await Task.Delay(1);
            }
        }
    }

    private async Task SendPackets()
    {
        if (!this._packetQueue.TryDequeue(out var task))
            return;

        if (task.Token is { IsCancellationRequested: true })
            return;

        this.DispatchPacket(task.Packet);

        _ = Task.Run(() => task.Task.TrySetResult());
        await this.HandleOutgoingPacket(task.Packet);
    }

    private void DispatchPacket(IPacket packet)
    {
        var packetId = this.Data.Protocol.GetPacketId(packet.Type);

        var buffer = new PacketBuffer(this._useAnonymousNbt);
        buffer.WriteVarInt(packetId);
        packet.Write(buffer, this.Data);

        this._stream!.WritePacket(buffer);
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
        if (this._internalPacketHandler.HandlesIncoming(packetType))
            handlers.Add(this._internalPacketHandler.HandleIncoming);

        // Custom packet handlers
        if (this._packetHandlers.TryGetValue(packetType, out var customHandlers))
            handlers.AddRange(customHandlers);

        // Forces all packets to be read
        if (null != this.OnPacketReceived)
            handlers.Add(this.InvokeReceivePacketAsync);

        this._packetWaiters.TryGetValue(packetType, out var tsc);

        if (handlers.Count == 0 && tsc == null)
        {
            await buffer.DisposeAsync();
            return;
        }

        long size = buffer.ReadableBytes;
        try
        {
            var packet = factory(buffer, this.Data);
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
        await this._internalPacketHandler.HandleOutgoing(packet);

        _ = Task.Run(() =>
        {
            try
            {
                this.OnPacketSent?.Invoke(this, packet);
            }
            catch (Exception e)
            {
                Logger.Warn($"Error in custom packet handling: {e}");
            }
        });
    }

    private Task InvokeReceivePacketAsync(IPacket packet)
    {
        return Task.Run(() => this.OnPacketReceived?.Invoke(this, packet));
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this._cancellation.Cancel();
        this._streamLoop?.Wait();

        this._client?.Dispose();
        this._stream?.Close();
    }

    private record PacketSendTask(IPacket Packet, CancellationToken? Token, TaskCompletionSource Task);

    /// <summary>
    /// Requests the server status and closes the connection.
    /// Works only when <see cref="gameState"/> is <see cref="Core.Common.Protocol.GameState.Status"/>.
    /// </summary>
    /// <returns></returns>
    public static async Task<ServerStatus> RequestServerStatus(
        string             hostnameOrIp,
        ushort             port       = 25565,
        int                timeout    = 10000,
        ITcpClientFactory? tcpFactory = null)
    {
        var latest = await MinecraftData.FromVersion(LATEST_SUPPORTED_VERSION);
        var client = new MinecraftClient(
            latest,
            Session.OfflineSession("RequestStatus"),
            hostnameOrIp,
            port,
            null, // api is not used for requesting server status
            tcpFactory);

        if (!await client.Connect(GameState.Status))
            throw new MineSharpHostException("Could not connect to server.");

        var timeoutCancellation  = new CancellationTokenSource();
        var taskCompletionSource = new TaskCompletionSource<ServerStatus>();

        client.On<StatusResponsePacket>(async packet =>
        {
            var json     = packet.Response;
            var response = ServerStatus.FromJToken(JToken.Parse(json), client.Data);
            taskCompletionSource.TrySetResult(response);

            // the server closes the connection 
            // after sending the StatusResponsePacket
            await client.Disconnect();
            client.Dispose();
        });

        await client.SendPacket(new StatusRequestPacket());
        await client.SendPacket(new PingRequestPacket(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()));

        timeoutCancellation.Token.Register(
            () => taskCompletionSource.TrySetCanceled(timeoutCancellation.Token));

        timeoutCancellation.CancelAfter(timeout);

        return await taskCompletionSource.Task;
    }

    /// <summary>
    /// Requests the server status and returns a <see cref="MinecraftData"/> object based on
    /// the version the server returned.
    /// </summary>
    /// <param name="hostname"></param>
    /// <param name="port"></param>
    /// <returns></returns>
    public static async Task<MinecraftData> AutodetectServerVersion(string hostname, ushort port)
    {
        var status = await RequestServerStatus(hostname, port);

        return await MinecraftData.FromVersion(status.Version);
    }
}
