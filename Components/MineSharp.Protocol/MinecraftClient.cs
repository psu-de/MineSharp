using MineSharp.Core.Logging;
using MineSharp.Core.Types.Enums;
using MineSharp.Data.Protocol;
using MineSharp.MojangAuth;
using MineSharp.Protocol.Handlers;
using System.Net;
using System.Net.Sockets;
using MineSharp.Data;

namespace MineSharp.Protocol {
    public class MinecraftClient {

        private static readonly Logger Logger = Logger.GetLogger();

        public string Version { get; private set; }
        public GameState GameState { get; private set; }
        public string IPAddress { get; private set; }
        public int Port { get; private set; }

        public Session Session { get; private set; }

        public delegate void ClientStringEvent(MinecraftClient client, string message);
        public delegate void ClientPacketEvent(MinecraftClient client, IPacketPayload packet);

        /// <summary>
        /// Fires when the client Disconnected
        /// </summary>
        public event ClientStringEvent? Disconnected;

        /// <summary>
        /// Fires when a packet has been received
        /// </summary>
        public event ClientPacketEvent? PacketReceived;

        /// <summary>
        /// Fires when a packet has been sent
        /// </summary>
        public event ClientPacketEvent? PacketSent;

        public IPacketHandler? PacketHandler;

        private readonly TcpClient _client;
        private MinecraftStream? _stream;

        private readonly CancellationTokenSource CancellationTokenSource;
        private readonly CancellationToken CancellationToken;
        private Task? _streamLoopTask;
        private readonly PacketFactory PacketFactory;

        private Queue<PacketSendTask> PacketQueue;
        internal int CompressionThreshold = -1;

        public MinecraftClient(string version, Session session, string host, int port) {
            this.Version = version;
            this.Session = session;
            this.GameState = GameState.HANDSHAKING;
            this.IPAddress = GetIpAddress(host);
            this.Port = port;
            this.CancellationTokenSource = new CancellationTokenSource();
            this.CancellationToken = CancellationTokenSource.Token;
            this.PacketQueue = new Queue<PacketSendTask>();
            this.PacketFactory = new PacketFactory(this);
            this.PacketHandler = GetPacketHandler(this.GameState);

            this._client = new TcpClient();
        }

        private string GetIpAddress(string host) {

            var type = Uri.CheckHostName(host);
            return type switch {
                UriHostNameType.Dns => 
                (Dns.GetHostEntry(host).AddressList.FirstOrDefault() 
                ?? throw new Exception($"Could not find ip for hostname ('{host}')")).ToString(),

                UriHostNameType.IPv4 => host,

                _ => throw new Exception("Hostname not supported: " + host)
            };

        }

        private IPacketHandler? GetPacketHandler(GameState state) {
            return state switch {
                GameState.HANDSHAKING => new HandshakePacketHandler(),
                GameState.LOGIN => new LoginPacketHandler(),
                GameState.PLAY => new PlayPacketHandler(),
                _ => null,
            };
        }

        /// <summary>
        /// Connects to the server and making the handshake
        /// </summary>
        /// <param name="nextState">In which <see cref="MineSharp.Protocol.GameState"/> the client and server should switch. Should be either <see cref="Protocol.GameState.LOGIN"/> or <see cref="Protocol.GameState.STATUS"/></param>
        /// <returns>A task that is completed as soon as the Handshake with the server is completed</returns>
        public async Task<bool> Connect (GameState nextState) {
            Logger.Debug("Connecting...");
            try {
                await this._client.ConnectAsync(this.IPAddress, this.Port, CancellationToken);

                // Start Reading / Writing Loops
                this._stream = new MinecraftStream(this._client.GetStream());
                this._streamLoopTask = Task.Run(async() => await this.StreamLoop(), CancellationToken);

                Logger.Info("Connected, starting handshake");
                await this.MakeHandshake(nextState);

                return true;
            } catch (Exception ex) {
                Logger.Error("Could not connect: " + ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Closes the connection on the client side without
        /// </summary>
        /// <param name="reason"></param>
        public void ForceDisconnect(string reason) {
            Logger.Debug("Forcing Disconnect: " + reason);
            this.CancellationTokenSource.Cancel();
            this._streamLoopTask?.Wait();
            this._client.Close();
            this.Disconnected?.Invoke(this, reason);
        }

        private async Task MakeHandshake(GameState nextState) {
            await this.SendPacket(
                new Data.Protocol.Handshaking.Serverbound.PacketSetProtocol(
                    MinecraftData.ProtocolVersion, 
                    this.IPAddress, 
                    (ushort)this.Port, 
                    (int)nextState));

            await (nextState switch {
                GameState.STATUS => this.SendPacket(new MineSharp.Data.Protocol.Status.Serverbound.PacketPingStart()),
                GameState.LOGIN => this.SendPacket(new Data.Protocol.Login.Serverbound.PacketLoginStart(this.Session.Username)),
                _ => throw new InvalidOperationException($"Next state '{nextState}' is not supported")
            });
        }

        byte[]? sharedSecret;
        public void SetEncryptionKey(byte[] key) {
            sharedSecret = key;
        }

        public void EnableEncryption() => this._stream!.EnableEncryption(sharedSecret!);

        public void SetCompressionThreshold(int compressionThreshold) {
            this.CompressionThreshold = compressionThreshold;
        }

        public void SetGameState(GameState newState) {
            this.GameState = newState;
            this.PacketHandler = GetPacketHandler(newState);
        }


        /// <summary>
        /// Sends a <see cref="Packet"/> to the Minecraft Server
        /// </summary>
        /// <param name="packet">Packet instance that will be sent</param>
        /// <returns>A task that will be completed when the packet has been written into the tcp stream</returns>
        public Task SendPacket(IPacketPayload packet, CancellationToken? cancellation = null) {
            if (packet == null) throw new ArgumentNullException();
            Logger.Debug3("Queueing packet: " + packet.GetType().Name);

            var sendTask = new PacketSendTask(cancellation, packet, new TaskCompletionSource());
            this.PacketQueue.Enqueue(sendTask);

            return sendTask.SendingTsc.Task;
        }


        private async Task StreamLoop() {
            Task readPacket () {
                int length = this._stream!.ReadVarInt();
                int uncompressedLength = 0;

                if (this.CompressionThreshold > 0) {
                    int r = 0;
                    uncompressedLength = this._stream.ReadVarInt(out r);
                    length -= r;
                }
                byte[] data = this._stream.Read(length);
                IPacketPayload? packet = this.PacketFactory.BuildPacket(data, uncompressedLength);
                if (packet != null) ThreadPool.QueueUserWorkItem((object? _) => {
                    //Logger.Debug3("Received packet: " + packet.GetType().Name); // Causes cpu usage spikes
                    try {
                        if (PacketHandler != null)
                            PacketHandler.HandleIncomming(packet, this).Wait(CancellationToken);
                        this.PacketReceived?.Invoke(this, packet);
                    } catch (Exception e) {
                        Logger.Error("There occurred an error while handling the packet: \n" + e.ToString());
                    }});
                return Task.CompletedTask;
            }

            while (!this.CancellationToken.IsCancellationRequested) {
                try {

                    if (this.GameState != GameState.PLAY) {
                        if (this._client.Available > 0) await readPacket();
                        await Task.Delay(1, CancellationToken);
                    } else {
                        while (this._client.Available > 0) { await readPacket(); }
                        await Task.Delay(1, CancellationToken);
                    }

                    if (this.PacketQueue.Count == 0) continue;
                    // Writing
                    PacketSendTask packetTask = this.PacketQueue.Dequeue();

                    if (packetTask.CancellationToken.HasValue && packetTask.CancellationToken.Value.IsCancellationRequested) {
                        packetTask.SendingTsc.SetCanceled(packetTask.CancellationToken.Value);
                        continue;
                    }

                    if (packetTask.Packet == null) // https://github.com/psu-de/MineSharp/issues/8#issue-1315635361
                    {
                        // for now just ignore the packet,
                        // since i have no idea why this happens
                        if (packetTask.SendingTsc != null)
                            packetTask.SendingTsc.TrySetCanceled();
                        continue;
                    }

                    var packetBuffer = PacketFactory.WritePacket(packetTask.Packet);

                    this._stream!.DispatchPacket(packetBuffer);

                    packetTask.SendingTsc.TrySetResult();
                    ThreadPool.QueueUserWorkItem((object? _) => {
                        try {
                            if (PacketHandler != null)
                                PacketHandler.HandleOutgoing(packetTask.Packet, this).Wait(CancellationToken);
                            PacketSent?.Invoke(this, packetTask.Packet);
                        } catch (Exception e) {
                            Logger.Error("Error while handling sent event of packet: \n" + e.ToString());
                        }
                    });
                } catch (Exception e) {
                    Logger.Error("Error in readLoop: " + e.ToString());
                }
            }
        }

        private struct PacketSendTask {
            public CancellationToken? CancellationToken { get; set; }
            public IPacketPayload Packet { get; set; }
            public TaskCompletionSource SendingTsc { get; set; }

            public PacketSendTask(CancellationToken? cancellationToken, IPacketPayload packet, TaskCompletionSource tsc) {
                this.CancellationToken = cancellationToken;
                this.Packet = packet;
                this.SendingTsc = tsc;
            }
        }
    }
}