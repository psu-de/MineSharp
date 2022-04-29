using MineSharp.Bot.Enums;
using MineSharp.Core.Logging;
using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;
using MineSharp.Core.Versions;
using MineSharp.Data.Blocks;
using MineSharp.Data.Entities;
using MineSharp.Data.Windows;
using MineSharp.MojangAuth;
using MineSharp.Protocol;
using MineSharp.Protocol.Packets;
using MineSharp.Protocol.Packets.Serverbound.Play;

namespace MineSharp.Bot {
    public partial class Bot {

        private static Logger Logger = Logger.GetLogger();

        public struct BotOptions {
            public string Version;
            public string UsernameOrEmail;
            public string? Password;
            public bool? Offline;
            public string Host;
            public ushort? Port;
        }

        public MinecraftVersion Version { get; private set; }
        public MinecraftClient Client { get; private set; }
        public BotOptions Options { get; private set; }
        public Session Session { get; private set; }


        #region Events 

        public delegate void BotEmptyEvent();
        public delegate void BotChatEvent(Chat chat);
        public delegate void BotItemEvent(Data.Items.Item? item);

        #endregion

        private Dictionary<Type, TaskCompletionSource<Packet>> packetWaiters = new Dictionary<Type, TaskCompletionSource<Packet>>();


        public Bot(BotOptions options) {
            this.Options = options;
            this.Version = new MinecraftVersion(this.Options.Version);

            if (this.Options.Offline == true) {
                this.Session = Session.OfflineSession(this.Options.UsernameOrEmail);
            } else {
                if (this.Options.Password == null) throw new Exception("Password cannot be null when Offline=false");
                this.Session = Session.Login(this.Options.UsernameOrEmail, this.Options.Password).GetAwaiter().GetResult();
                Logger.Info("UUID: " + this.Session.UUID);
            }

            this.Client = new MinecraftClient(this.Options.Version, this.Session, this.Options.Host, this.Options.Port ?? 25565);
            this.Client.Events.PacketReceived += Events_PacketReceived;

            this.LoadWindows();
            this.LoadEntities();
            this.LoadWorld();
            this.LoadMovements();
        }

        private partial void LoadWindows();
        private partial void LoadEntities();
        private partial void LoadWorld();
        private partial void LoadMovements();

        public async Task<bool> Connect() {
            this.Health = 20;
            return await this.Client.Connect(GameState.LOGIN);
        }

        public async Task WaitUntilLoaded() {
            while (!isPlayerLoaded) {
                await Task.Delay(10);
            }
        }

        public Task<Packet> WaitForPacket<T>() where T : Packet {
            Type packetType = typeof(T);
            if (!packetWaiters.TryGetValue(packetType, out var task)) {
                TaskCompletionSource<Packet> tsc = new TaskCompletionSource<Packet>();
                this.packetWaiters.Add(packetType, tsc);

                return tsc.Task ?? throw new ArgumentNullException();
            } else return task.Task ?? throw new ArgumentNullException();
        }

        private void Events_PacketReceived(MinecraftClient client, Packet packet) {

            Type packetType = packet.GetType();
            if (packetWaiters.TryGetValue(packetType, out var tsc)) {
                tsc.TrySetResult(packet);
                packetWaiters.Remove(packetType);
            }

            switch (packet) {

                // Base
                case Protocol.Packets.Clientbound.Play.JoinGamePacket p_0x26: handleJoinGame(p_0x26); break;
                case Protocol.Packets.Clientbound.Play.DeathCombatEventPacket p_0x35: handleDeathCombat(p_0x35); break;
                case Protocol.Packets.Clientbound.Play.DestroyEntitiesPacket p_0x3A: handleDespawnEntity(p_0x3A); break;
                case Protocol.Packets.Clientbound.Play.RespawnPacket p_0x3D: handleRespawn(p_0x3D); break;
                case Protocol.Packets.Clientbound.Play.UpdateHealthPacket p_0x52: handleUpdateHealth(p_0x52); break;
                case Protocol.Packets.Clientbound.Play.HeldItemChangePacket p_0x48: handleHeldItemChange(p_0x48); break;

                // Entities 
                case Protocol.Packets.Clientbound.Play.SpawnLivingEntityPacket p_0x02: handleSpawnLivingEntity(p_0x02); break;
                case Protocol.Packets.Clientbound.Play.SpawnPlayerPacket p_0x04: handleSpawnPlayer(p_0x04); break;
                case Protocol.Packets.Clientbound.Play.EntityPositionPacket p_0x29: handleEntityPosition(p_0x29); break;
                case Protocol.Packets.Clientbound.Play.EntityPositionAndRotationPacket p_0x2A: handleEntityPositionAndRotation(p_0x2A); break;
                case Protocol.Packets.Clientbound.Play.EntityRotationPacket p_0x2B: handleEntityRotation(p_0x2B); break;
                case Protocol.Packets.Clientbound.Play.PlayerInfoPacket p_0x36: handlePlayerInfo(p_0x36); break;
                case Protocol.Packets.Clientbound.Play.PlayerPositionAndLookPacket p_0x38: handlePlayerPositionAndLook(p_0x38); break;
                case Protocol.Packets.Clientbound.Play.EntityVelocityPacket p_0x4F: handleUpdateEntityVelocity(p_0x4F); break;

                // World
                case Protocol.Packets.Clientbound.Play.ChunkDataAndLightUpdatePacket p_0x22: handleChunkDataAndLightUpdate(p_0x22); break;
                case Protocol.Packets.Clientbound.Play.UnloadChunkPacket p_0x1D: handleUnloadChunk(p_0x1D); break;
                case Protocol.Packets.Clientbound.Play.BlockChangePacket p_0x0C: handleBlockUpdate(p_0x0C); break;
                case Protocol.Packets.Clientbound.Play.MultiBlockChangePacket p_0x3F: handleMultiBlockChange(p_0x3F); break;

                // Windows
                case Protocol.Packets.Clientbound.Play.WindowItemsPacket p_0x14: handleWindowItems(p_0x14); break;
                case Protocol.Packets.Clientbound.Play.SetSlotPacket p_0x16: handleSetSlot(p_0x16); break;

            }
        }



        #region Minecraft Interactions

        public Task<MineBlockStatus> MineBlock(Block block, BlockFace? face = null, CancellationToken? cancellation = null) {
            return Task.Run(async () => {

                if (!block.Info.Diggable) return MineBlockStatus.NotDiggable;
                if (!World.IsBlockLoaded(block.Position)) return MineBlockStatus.BlockNotLoaded;


                if (face == null) {
                    // TODO: Maybe rausfinden wie des geht   
                }

                if (cancellation?.IsCancellationRequested ?? false) return MineBlockStatus.Cancelled;

                var packet = new MineSharp.Protocol.Packets.Serverbound.Play.PlayerDiggingPacket(DiggingStatus.StartedDigging, block.Position, face ?? BlockFace.Top);

                await this.Client.SendPacket(packet);

                int time = block.Info.CalculateBreakingTime(this.HeldItem?.Info);

                CancellationTokenSource cancelToken = new CancellationTokenSource();

                cancellation?.Register(() => cancelToken.Cancel());

                Task<Protocol.Packets.Clientbound.Play.AcknowledgePlayerDiggingPacket?> sendAgain = Task.Run(async () => {
                    await Task.Delay(time, cancelToken.Token);
                    if (cancelToken.Token.IsCancellationRequested) return null;

                    packet.Status = DiggingStatus.FinishedDigging;
                    await this.Client.SendPacket(packet);
                    return await this.WaitForPacket<Protocol.Packets.Clientbound.Play.AcknowledgePlayerDiggingPacket>() as Protocol.Packets.Clientbound.Play.AcknowledgePlayerDiggingPacket;
                });

                var ack = await this.WaitForPacket<Protocol.Packets.Clientbound.Play.AcknowledgePlayerDiggingPacket>() as Protocol.Packets.Clientbound.Play.AcknowledgePlayerDiggingPacket;
                if (cancellation?.IsCancellationRequested ?? false) {
                    cancelToken.Cancel();
                    await this.Client.SendPacket(new PlayerDiggingPacket(DiggingStatus.CancelledDigging, block.Position, face ?? BlockFace.Top));
                    return MineBlockStatus.Cancelled;
                }

                if (ack.Status != DiggingStatus.StartedDigging) {
                    return MineBlockStatus.Failed;
                }

                var secondPacket = await sendAgain;
                if (secondPacket == null || !secondPacket.Successful) {

                    return MineBlockStatus.Failed;
                }

                return MineBlockStatus.Finished;
            });
        }

        /// <summary>
        /// Respawns the bot
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Thrown when bot is still alive</exception>
        public Task Respawn() {
            if (!this.IsAlive) {

                return this.Client.SendPacket(new ClientStatusPacket(ClientStatusPacket.ClientStatusAction.PerformRespawn)).ContinueWith((x) => this.IsAlive = true);

            } else throw new NotSupportedException();
        }


        /// <summary>
        /// Changes the selected hotbar slot 
        /// </summary>
        /// <param name="slot">Index in the hotbar (0-8)</param>
        /// <returns>A task that will be completed when the operation is finshed</returns>
        public Task SelectHotbarSlot(byte slot) {

            if (slot < 0 || slot > 8) throw new IndexOutOfRangeException();

            return this.Client.SendPacket(new Protocol.Packets.Serverbound.Play.HeldItemChangePacket(slot)).ContinueWith((x) => {
                this.SelectedHotbarIndex = slot;
                this.HeldItemChanged?.Invoke(this.HeldItem);
                return Task.CompletedTask;
            });
        }

        public Task Attack(Entity entity) {
            // TODO: Cooldown
            if (entity.Position.DistanceSquared(this.BotEntity.Position) > 36) throw new InvalidOperationException("Too far");

            var packet = new Protocol.Packets.Serverbound.Play.InteractEntityPacket(entity.Id, InteractEntityPacket.InteractMode.Attack, false); // TODO: Change sneaking
            return this.Client.SendPacket(packet);
        }

        public Task Chat(string message) {
            var packet = new Protocol.Packets.Serverbound.Play.ChatMessagePacket(message);
            return this.Client.SendPacket(packet);
        }

        public async Task<Window?> OpenChest(Block block) {
            if (block.Info.Id != BlockType.Chest || block.Info.Id != BlockType.TrappedChest || block.Info.Id == BlockType.EnderChest) return null;

            var packet = new PlayerBlockPlacementPacket(0, block.Position, BlockFace.Top, 0.5f, 0.5f, 0.5f, false); // TODO: Block Face 
            await this.Client.SendPacket(packet);

            var windowPacket = await WaitForPacket<Protocol.Packets.Clientbound.Play.OpenWindowPacket>() as Protocol.Packets.Clientbound.Play.OpenWindowPacket;

            Window? window = GetWindow(windowPacket.WindowID);
            if (window != null) { // Update window
                throw new NotImplementedException();
            }

            window = Window.CreateWindowById(windowPacket.WindowID);
            this.OpenWindows.Add(windowPacket.WindowID, window);
            return window;
        }


        #endregion

    }
}