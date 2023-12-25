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
using Newtonsoft.Json;
using NLog;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;

namespace MineSharp.Protocol;

public sealed class MinecraftClient : IDisposable
{
    public const string LATEST_SUPPORTED_VERSION = "1.20.1";
    
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    public delegate Task AsyncPacketHandler(IPacket packet);
    public delegate Task AsyncPacketHandler<in T>(T packet) where T : IPacket;

    private readonly MinecraftData _data;
    private readonly CancellationTokenSource _cancellation;
    private readonly TcpClient _client;

    private readonly Queue<(PacketType, PacketBuffer)> _bundledPackets;
    private readonly ConcurrentQueue<PacketSendTask> _packetQueue;
    private readonly IDictionary<PacketType, IList<AsyncPacketHandler>> _packetHandlers;
    private readonly IDictionary<PacketType, TaskCompletionSource<object>> _packetWaiters;
    private readonly TaskCompletionSource _gameJoinedTsc;

    private bool _bundlePackets;
    private IPacketHandler _internalPacketHandler;
    private MinecraftStream? _stream;
    private Task? _streamLoop;

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
    
    public Session Session { get; }
    public IPAddress IP { get; }
    public ushort Port { get; }
    public GameState GameState { get; private set; }

    public MinecraftClient(MinecraftData data, Session session, string hostnameOrIp, ushort port)
    {
        this._data = data;
        this._client = new TcpClient();
        this._packetQueue = new ConcurrentQueue<PacketSendTask>();
        this._cancellation = new CancellationTokenSource();
        this._internalPacketHandler = new HandshakePacketHandler(this);
        this._packetHandlers = new Dictionary<PacketType, IList<AsyncPacketHandler>>();
        this._packetWaiters = new Dictionary<PacketType, TaskCompletionSource<object>>();
        this._gameJoinedTsc = new TaskCompletionSource();
        this._bundledPackets = new Queue<(PacketType, PacketBuffer)>();

        this.Session = session;
        this.IP = IPHelper.ResolveHostname(hostnameOrIp);
        this.Port = port;
        this.GameState = GameState.Handshaking;
    }

    /// <summary>
    /// Connects to the minecraft sever.
    /// </summary>
    /// <param name="nextState">The state to transition to during the handshaking process.</param>
    /// <returns>A task that resolves once connected. Results in true when connected successfully, false otherwise.</returns>
    public async Task<bool> Connect(GameState nextState)
    {
        Logger.Debug($"Connecting to {this.IP}:{this.Port}.");

        try
        {
            await this._client.ConnectAsync(this.IP, this.Port, this._cancellation.Token);
            this._stream = new MinecraftStream(this._client.GetStream());
            
            this.StreamLoop();
            Logger.Info("Connected, starting handshake...");
            await HandshakeProtocol.PerformHandshake(this, nextState, this._data);
            
        } catch (SocketException ex)
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
    /// Registers an <see cref="handler"/> that will be called whenever an packet of type <see cref="T"/> is received
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
    /// Waits until the client jumps into the Play <see cref="GameState"/>
    /// </summary>
    /// <returns></returns>
    public Task WaitForGame()
        => this._gameJoinedTsc.Task;

    internal void UpdateGameState(GameState next)
    {
        this.GameState = next;

        this._internalPacketHandler = next switch {
            GameState.Handshaking => new HandshakePacketHandler(this),
            GameState.Login => new LoginPacketHandler(this, this._data),
            GameState.Status => new StatusPacketHandler(this),
            GameState.Configuration => new ConfigurationPacketHandler(this, this._data),
            GameState.Play => new PlayPacketHandler(this, this._data),
            _ => throw new UnreachableException()
        };

        if (next == GameState.Play)
            this._gameJoinedTsc.TrySetResult();
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
                    p => this.HandleIncomingPacket(p.Item1, p.Item2))
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
                } catch (Exception ex)
                {
                    Logger.Error(ex, "Encountered error in stream loop");
                }
            }
        }, this._cancellation.Token);
    }
    
    private async Task ReceivePackets()
    {
        while (this._client.Available > 0 && !this._cancellation.IsCancellationRequested)
        {
            var buffer = this._stream!.ReadPacket();
            var packetId = buffer.ReadVarInt();
            var packetType = this._data.Protocol.FromPacketId(PacketFlow.Clientbound, this.GameState, packetId);

            if (this._bundlePackets)
                this._bundledPackets.Enqueue((packetType, buffer));
            else await this.HandleIncomingPacket(packetType, buffer);
            
            if (this.GameState != GameState.Play)
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
        var packetId = this._data.Protocol.GetPacketId(packet.Type); 
        
        var buffer = new PacketBuffer();
        buffer.WriteVarInt(packetId);
        packet.Write(buffer, this._data);

        this._stream!.WritePacket(buffer);
    }

    private async Task HandleIncomingPacket(PacketType packetType, PacketBuffer buffer)
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
            var packet = factory(buffer, this._data);
            await buffer.DisposeAsync();
            
            _ = Task.Run(async () =>
            {
                tsc?.TrySetResult(packet);
                var tasks = handlers
                    .Select(task => task(packet))
                    .ToArray();
                
                try
                {
                    await Task.WhenAll(tasks);
                } catch (Exception)
                {
                    foreach (var exception in tasks.Where(x => x.Exception != null))
                    {
                        Logger.Warn($"Error in custom packet handling: {exception.Exception}");
                    }
                }
            });
        } catch (EndOfStreamException e)
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
            } catch (Exception e)
            {
                Logger.Warn($"Error in custom packet handling: {e}");
            }
        });
    }

    private Task InvokeReceivePacketAsync(IPacket packet)
    {
        if (null == OnPacketReceived)
            return Task.CompletedTask;

        return Task.Factory.FromAsync(
            (callback, obj) => this.OnPacketReceived.BeginInvoke(this, packet, callback, obj),
            this.OnPacketReceived.EndInvoke,
            null);
    }
    
    public void Dispose()
    {
        this._cancellation.Cancel();
        this._streamLoop?.Wait();
        
        this._client.Dispose();
        this._stream?.Close();
    }

    
    private record PacketSendTask(IPacket Packet, CancellationToken? Token, TaskCompletionSource Task);

    
    
    /// <summary>
    /// Requests the server status and closes the connection.
    /// Works only when <see cref="GameState"/> is <see cref="Core.Common.Protocol.GameState.Status"/>.
    /// </summary>
    /// <returns></returns>
    public static async Task<ServerStatusResponseBlob> RequestServerStatus(
        string hostnameOrIp,
        ushort port,
        int timeout = 10000)
    {
        var latest = MinecraftData.FromVersion(LATEST_SUPPORTED_VERSION);
        var client = new MinecraftClient(
            latest,
            Session.OfflineSession("RequestStatus"),
            hostnameOrIp,
            port);

        if (!await client.Connect(GameState.Status))
            throw new MineSharpHostException("Could not connect to server.");

        var timeoutCancellation = new CancellationTokenSource();
        var taskCompletionSource = new TaskCompletionSource<ServerStatusResponseBlob>();

        client.On<StatusResponsePacket>(async packet =>
        {
            var json = packet.Response;
            var response = JsonConvert.DeserializeObject<ServerStatusResponseBlob>(json)!;
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
        return await RequestServerStatus(hostname, port)
            .ContinueWith(
                prev => MinecraftData.FromVersion(prev.Result.Version.Name));
    }
}