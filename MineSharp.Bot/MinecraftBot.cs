using MineSharp.Bot.Modules;
using MineSharp.Core;
using MineSharp.Core.Logging;
using MineSharp.Core.Types;
using MineSharp.Core.Versions;
using MineSharp.MojangAuth;
using MineSharp.Protocol;
using MineSharp.Protocol.Packets;
using MineSharp.Windows;

using Item = MineSharp.Core.Types.Item;

namespace MineSharp.Bot {
    public partial class MinecraftBot {

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

        public delegate void BotEmptyEvent(MinecraftBot sender);
        public delegate void BotChatEvent(MinecraftBot sender, Chat chat);
        public delegate void BotItemEvent(MinecraftBot sender, Item? item);

        public delegate void BotPlayerEvent(MinecraftBot sender, MinecraftPlayer entity);
        public delegate void BotEntityEvent(MinecraftBot sender, Entity entity);

        public delegate void BotWindowEvent(MinecraftBot sender, Window window);

        #endregion

        private Dictionary<Type, object> packetWaiters = new Dictionary<Type, object>();
        private Dictionary<Type, List<Func<Packet, Task>>> PacketHandlers = new Dictionary<Type, List<Func<Packet, Task>>>();
        private Task TickLoopTask;

        public List<Module> Modules = new List<Module>();
        private List<TickedModule> TickedModules = new List<TickedModule>();

        public BaseModule BaseModule;

        public EntityModule EntityModule;
        public PlayerModule PlayerModule;
        public WorldModule WorldModule;
        public PhysicsModule PhysicsModule;
        public WindowsModule WindowsModule;

        public MinecraftBot(BotOptions options) {
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
            this.Client.PacketReceived += Events_PacketReceived;
            

        }

        public async Task LoadModule(Module module) {

            this.Modules.Add(module);
            if (module is TickedModule)
                this.TickedModules.Add((TickedModule)module);

            await module.Initialize();
            Logger.Info("Loaded module: " + module.GetType().Name);
        }

        private async Task TickLoop () {
            while (true) {
                var start = DateTime.Now;

                var tasks = new List<Task>();

                foreach (var tickedModule in TickedModules) {
                    if (tickedModule.IsEnabled) {
                        tasks.Add(tickedModule.Tick());
                    }
                }
                await Task.WhenAll(tasks);

                var deltaTime = MinecraftConst.TickMs - (int)(DateTime.Now - start).TotalMilliseconds;
                if (deltaTime < 0) {
                    Logger.Warning($"Ticked modules taking too long, {-deltaTime}ms behind");
                }else    
                    await Task.Delay(deltaTime);
            }
        }


        private async Task LoadModules () {
            this.BaseModule = new BaseModule(this);
            this.EntityModule = new EntityModule(this);
            this.PlayerModule = new PlayerModule(this);
            this.PhysicsModule = new PhysicsModule(this);
            this.WorldModule = new WorldModule(this);
            this.WindowsModule = new WindowsModule(this);

            await Task.WhenAll(new Task[] {
                LoadModule(this.BaseModule),
                LoadModule(this.EntityModule),
                LoadModule(this.PlayerModule),
                LoadModule(this.PhysicsModule),
                LoadModule(this.WorldModule),
                LoadModule(this.WindowsModule),
            });
        }


        /// <summary>
        /// Connects the bot to the server
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Connect() {
            if (!await this.Client.Connect(GameState.LOGIN)) {
                return false;
            }

            await LoadModules();

            TickLoopTask = TickLoop();
            return true;
        }

        /// <summary>
        /// Waits for a specific packet from the server
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [BotFunction("Basic", "Waits for a specific packet from the server")]
        public Task<T> WaitForPacket<T>() where T : Packet {
            Type packetType = typeof(T);
            if (!packetWaiters.TryGetValue(packetType, out var task)) {
                TaskCompletionSource<T> tsc = new TaskCompletionSource<T>();
                this.packetWaiters.Add(packetType, tsc);

                return tsc.Task ?? throw new ArgumentNullException();
            } else return (((TaskCompletionSource<T>?)task)?.Task) ?? throw new ArgumentNullException();
        }

        [BotFunction("Basic", "Calls handler every time a specific packet is received")]
        public void On<T>(Func<T, Task> handler) where T : Packet {
            if (PacketHandlers.ContainsKey(typeof(T))) {
                PacketHandlers[typeof(T)].Add((Packet p) => handler((T)p));
            } else {
                PacketHandlers.Add(typeof(T), new List<Func<Packet, Task>>() { (Packet p) => handler((T)p) });
            }
        }

        private void Events_PacketReceived(MinecraftClient client, Packet packet) {

            Type packetType = packet.GetType();
            if (packetWaiters.TryGetValue(packetType, out var tsc)) {
                if (null == typeof(TaskCompletionSource<>).MakeGenericType(packetType).GetMethod("TrySetResult")?.Invoke(tsc, new[] { packet })) {
                    throw new InvalidOperationException();
                }
                
                packetWaiters.Remove(packetType);
            }

            List<Task> tasks = new List<Task>();
            if (PacketHandlers.TryGetValue(packetType, out var handlers)) {
                foreach (var handler in handlers) {
                    tasks.Add(handler(packet));
                }
            }

            Task.WaitAll(tasks.ToArray());
        }



        #region Public Methods

        [BotFunction("Basic", "Respawns the bot. Only possible when the bot is dead.")]
        public Task Respawn() => BaseModule.Respawn();

        [BotFunction("Basic", "Attacks a given entity")]
        public Task Attack(Entity entity) => BaseModule.Attack(entity);

        [BotFunction("Basic", "Sends a chat message to the server")]
        public Task Chat(string message) => BaseModule.Chat(message);

        #endregion

    }
}