using MineSharp.Auth;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Protocol;
using MineSharp.Data;
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

    private readonly Queue<PacketSendTask> _packetQueue;
    private readonly IDictionary<Guid, IList<AsyncPacketHandler>> _packetHandlers;
    private readonly IDictionary<Guid, TaskCompletionSource<object>> _packetWaiters;
    private readonly TaskCompletionSource _gameJoinedTsc;

    private IPacketHandler _internalPacketHandler;
    private MinecraftStream? _stream;
    private Task? _streamLoop;

    public event Events.ClientPacketEvent? OnPacketReceived;
    public event Events.ClientPacketEvent? OnPacketSent;
    public event Events.ClientStringEvent? OnDisconnected;
    
    public Session Session { get; }
    public IPAddress IP { get; }
    public ushort Port { get; }
    public GameState GameState { get; private set; }

    public MinecraftClient(MinecraftData data, Session session, string hostnameOrIp, ushort port)
    {
        this._data = data;
        this._client = new TcpClient();
        this._packetQueue = new Queue<PacketSendTask>();
        this._cancellation = new CancellationTokenSource();
        this._internalPacketHandler = new HandshakePacketHandler(this);
        this._packetHandlers = new Dictionary<Guid, IList<AsyncPacketHandler>>();
        this._packetWaiters = new Dictionary<Guid, TaskCompletionSource<object>>();
        this._gameJoinedTsc = new TaskCompletionSource();

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

    public void On<T>(AsyncPacketHandler<T> handler) where T : IPacket
    {
        var key = typeof(T).GUID;

        if (!this._packetHandlers.TryGetValue(key, out var handlers))
        {
            handlers = new List<AsyncPacketHandler>();
            this._packetHandlers.Add(key, handlers);
        }

        handlers.Add(p => handler((T)p));
    }

    public Task<T> WaitForPacket<T>() where T : IPacket
    {
        var packetType = typeof(T).GUID;
        if (!this._packetWaiters.TryGetValue(packetType, out var task))
        {
            var tsc = new TaskCompletionSource<object>();
            this._packetWaiters.Add(packetType, tsc);

            return tsc.Task.ContinueWith(prev => (T)prev.Result);
        }

        return task.Task.ContinueWith(prev => (T)prev.Result);
    }
    
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
            long size = buffer.ReadableBytes;

            var factory = PacketPalette.GetFactory(packetType);
            try
            {
                var packet = factory?.Invoke(buffer, this._data);
                if (packet != null)
                    await this.HandleIncomingPacket(packet);

                if (this.GameState != GameState.Play)
                {
                    await Task.Delay(1);
                }
            } catch (EndOfStreamException)
            {
                Console.WriteLine($"Error reading packet with id {packetId}, assumed it was {packetType}. Packet was {size} bytes.");
            }
        }
    }

    private async Task SendPackets()
    {
        if (this._packetQueue.Count == 0)
            return;

        var task = this._packetQueue.Dequeue();

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

    private async Task HandleIncomingPacket(IPacket packet)
    {
        await this._internalPacketHandler.HandleIncoming(packet);

        _ = Task.Run(async () =>
        {
            try
            {
                var key = packet.GetType().GUID;
                
                if (this._packetWaiters.TryGetValue(key, out var tsc))
                    tsc.TrySetResult(packet);
                
                if (this._packetHandlers.TryGetValue(key, out var handlers))
                {
                    foreach (var handler in handlers)
                        await handler(packet);
                }

                this.OnPacketReceived?.Invoke(this, packet);
            } catch (Exception e)
            {
                Logger.Warn($"Error in custom packet handling: {e}");
            }
        });
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