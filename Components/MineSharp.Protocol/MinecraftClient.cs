using MineSharp.Core.Logging;
using MineSharp.Core.Types.Enums;
using MineSharp.Core.Versions;
using MineSharp.Data.Protocol;
using MineSharp.MojangAuth;
using MineSharp.Protocol.Handlers;
using System.Net;
using System.Net.Sockets;

namespace MineSharp.Protocol {
    public class MinecraftClient {

        private static Logger Logger = Logger.GetLogger();

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

        private struct PacketSendTask {
            public CancellationToken? CancellationToken;
            public IPacketPayload Packet;
            public TaskCompletionSource SendingTsc;

            public PacketSendTask(CancellationToken? cancellationToken, IPacketPayload packet, TaskCompletionSource tsc) {
                this.CancellationToken = cancellationToken;
                this.Packet = packet;
                this.SendingTsc = tsc;
            }
        }


        private TcpClient _client;
        private MinecraftStream? Stream;


        private CancellationTokenSource CancellationTokenSource;
        private CancellationToken CancellationToken;
        private Task? streamLoopTask;
        private PacketFactory PacketFactory;
        private IPacketHandler? PacketHandler;

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
                await this._client.ConnectAsync(this.IPAddress, this.Port);

                // Start Reading / Writing Loops
                this.Stream = new MinecraftStream(this._client.GetStream());
                this.streamLoopTask = Task.Run(async() => await this.streamLoop());

                Logger.Info("Connected, starting handshake");
                await this.makeHandshake(nextState);

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
            this.streamLoopTask?.Wait();
            this._client.Close();
            this.Disconnected?.Invoke(this, reason);
        }

        private async Task makeHandshake(GameState nextState) {
            await this.SendPacket(new Data.Protocol.Handshaking.Serverbound.PacketSetProtocol(ProtocolVersion.GetVersionNumber(this.Version), this.IPAddress, (ushort)this.Port, (int)nextState));

            if (nextState == GameState.STATUS) {
                await this.SendPacket(new MineSharp.Data.Protocol.Status.Serverbound.PacketPingStart());
            } else if (nextState == GameState.LOGIN) {
                await this.SendPacket(new Data.Protocol.Login.Serverbound.PacketLoginStart(this.Session.Username));
            }
        }

        byte[]? sharedSecret;
        public void SetEncryptionKey(byte[] key) {
            sharedSecret = key;
        }

        public void EnableEncryption() => this.Stream!.EnableEncryption(sharedSecret!);

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


        private async Task streamLoop() {
            Task readPacket () {
                int length = this.Stream!.ReadVarInt();
                int uncompressedLength = 0;

                if (this.CompressionThreshold > 0) {
                    int r = 0;
                    uncompressedLength = this.Stream.ReadVarInt(out r);
                    length -= r;
                }
                byte[] data = this.Stream.Read(length);
                IPacketPayload? packet = this.PacketFactory.BuildPacket(data, uncompressedLength);
                if (packet != null) ThreadPool.QueueUserWorkItem(async (object? _) => {
                    //Logger.Debug3("Received packet: " + packet.GetType().Name); // Causes cpu usage spikes
                    try {
                        if (PacketHandler != null)
                            await PacketHandler.HandleIncomming(packet, this);
                        this.PacketReceived?.Invoke(this, packet);
                    } catch (Exception e) {
                        Logger.Error("There occured an error while handling the packet: \n" + e.ToString());
                    }});
                return Task.CompletedTask;
            }

            while (!this.CancellationToken.IsCancellationRequested) {
                try {

                    if (this.GameState != GameState.PLAY) {
                        if (this._client.Available > 0) await readPacket();
                        await Task.Delay(1);
                    } else {
                        while (this._client.Available > 0) { await readPacket(); }
                        await Task.Delay(1);
                    }

                    if (this.PacketQueue.Count > 0) { // Writing
                        PacketSendTask packetTask = this.PacketQueue.Dequeue();

                        if (packetTask.CancellationToken.HasValue && packetTask.CancellationToken.Value.IsCancellationRequested) {
                            packetTask.SendingTsc.SetCanceled(packetTask.CancellationToken.Value);
                            continue;
                        }

                        var packetBuffer = PacketFactory.WritePacket(packetTask.Packet);

                        this.Stream!.DispatchPacket(packetBuffer);

                        packetTask.SendingTsc.TrySetResult();
                        ThreadPool.QueueUserWorkItem(async (object? _) => {
                            try {
                                if (PacketHandler != null)
                                    await PacketHandler.HandleOutgoing(packetTask.Packet, this);
                                PacketSent?.Invoke(this, packetTask.Packet);
                            } catch (Exception e) {
                                Logger.Error("Error while handling sent event of packet: \n" + e.ToString());
                            }
                        });
                    }

                } catch (Exception e) {
                    Logger.Error("Error in readLoop: " + e.ToString());
                }
            }
        }
    }
}