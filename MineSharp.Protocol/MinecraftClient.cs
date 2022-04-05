using MineSharp.Core.Logging;
using MineSharp.Core.Versions;
using MineSharp.MojangAuth;
using MineSharp.Protocol.Packets;
using MineSharp.Protocol.Packets.Serverbound.Handshaking;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace MineSharp.Protocol {
    public class MinecraftClient {

        public string Version { get; private set; }
        public GameState GameState { get; private set; }
        public string Hostname { get; private set; }
        public int Port { get; private set; }

        public Session Session { get; private set; }

        public Events.MinecraftClientEvents Events { get; private set; }

        private Logger Logger;
        private TcpClient _client;
        private MinecraftStream Stream;


        private CancellationTokenSource CancellationTokenSource;
        private CancellationToken CancellationToken;
        private Task? streamLoopTask;
        private PacketFactory PacketFactory;

        private Queue<(Packet, TaskCompletionSource<bool>)> PacketQueue;
        private int CompressionThreshold = -1;

        public MinecraftClient(string version, Session session, string host, int port) {
            this.Version = version;
            this.Session = session;
            this.GameState = GameState.HANDSHAKING;
            this.Hostname = host;
            this.Port = port;
            this.Logger = Logger.GetLogger();
            this.CancellationTokenSource = new CancellationTokenSource();
            this.CancellationToken = CancellationTokenSource.Token;
            this.PacketQueue = new Queue<(Packet, TaskCompletionSource<bool>)>();
            this.Events = new Events.MinecraftClientEvents();
            this.PacketFactory = new PacketFactory(this);

            Packet.Initialize();

            this._client = new TcpClient();
        }

        /// <summary>
        /// Connects to the server and making the handshake
        /// </summary>
        /// <param name="nextState">In which <see cref="MineSharp.Protocol.GameState"/> the client and server should switch. Should be either <see cref="Protocol.GameState.LOGIN"/> or <see cref="Protocol.GameState.STATUS"/></param>
        /// <returns>A task that is completed as soon as the Handshake with the server is completed</returns>
        public async Task<bool> Connect (GameState nextState) {
            this.Logger.Debug("Connecting...");
            try {
                await this._client.ConnectAsync(this.Hostname, this.Port);

                // Start Reading / Writing Loops
                this.Stream = new MinecraftStream(this._client.GetStream());
                this.streamLoopTask = Task.Run(async() => await this.streamLoop());

                this.Logger.Info("Connected, starting handshake");
                await this.makeHandshake(nextState);

                return true;
            } catch (Exception ex) {
                this.Logger.Error("Could not connect: " + ex.ToString());
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
            this.Events.InvokeClientDisconnected(this, reason);
        }

        private async Task makeHandshake(GameState nextState) {
            await this.SendPacket(new HandshakePacket(ProtocolVersion.GetVersionNumber(this.Version), this.Hostname, (ushort)this.Port, nextState));

            if (nextState == GameState.STATUS) {
                await this.SendPacket(new Packets.Serverbound.Status.RequestPacket());
            } else if (nextState == GameState.LOGIN) {
                await this.SendPacket(new Packets.Serverbound.Login.LoginStartPacket(this.Session.Username));
            }
        }

        byte[] sharedSecret;
        internal void SetEncryptionKey(byte[] key) {
            sharedSecret = key;
        }

        internal void EnableEncryption() => this.Stream.EnableEncryption(sharedSecret);

        internal void SetCompressionThreshold(int compressionThreshold) {
            this.CompressionThreshold = compressionThreshold;
        }

        internal void SetGameState(GameState newState) {
            this.GameState = newState;
        }


        /// <summary>
        /// Sends a <see cref="Packet"/> to the Minecraft Server
        /// </summary>
        /// <param name="packet">Packet instance that will be sent</param>
        /// <returns>A task that will be completed when the packet has been written into the tcp stream</returns>
        public Task SendPacket(Packet packet) {
            var tsc = new TaskCompletionSource<bool>();
            this.PacketQueue.Enqueue((packet, tsc));
            return tsc.Task;
        }


        private async Task streamLoop() {
            async void readPacket () {
                int length = this.Stream.ReadVarInt();
                int uncompressedLength = 0;

                if (this.CompressionThreshold > 0) {
                    int r = 0;
                    uncompressedLength = this.Stream.ReadVarInt(out r);
                    length -= r;
                }
                byte[] data = this.Stream.Read(length);
                Packet? packet = this.PacketFactory.BuildPacket(data, uncompressedLength);

                if (packet != null) ThreadPool.QueueUserWorkItem(async (object? _) => {

                    try {
                        await packet.Handle(this);
                    } catch (Exception e) {
                        Logger.Error("There occured an error while handling the packet: \n" + e.ToString());
                    }});
            }

            while (!this.CancellationToken.IsCancellationRequested) {
                try {

                    if (this.GameState != GameState.PLAY) {
                        if (this._client.Available > 0) readPacket();
                        await Task.Delay(1);
                    } else {
                        while (this._client.Available > 0) { readPacket(); }
                        await Task.Delay(1);
                    }

                    if (this.PacketQueue.Count > 0) { // Writing
                        (Packet packet, TaskCompletionSource<bool> sendTask) = this.PacketQueue.Dequeue();
                        PacketBuffer packetBuffer = new PacketBuffer();
                        packetBuffer.WriteVarInt(Packet.GetPacketId(packet.GetType()));
                        packet.Write(packetBuffer);

                        if (this.CompressionThreshold > 0) {
                            packetBuffer = PacketBuffer.Compress(packetBuffer, this.CompressionThreshold);
                        }

                        this.Stream.DispatchPacket(packetBuffer);


                        sendTask.TrySetResult(true);
                        ThreadPool.QueueUserWorkItem(async (object? _) => {
                            try {
                                await packet.Sent(this);
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