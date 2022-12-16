using MineSharp.Bot.Modules;
using MineSharp.Bot.Modules.Base;
using MineSharp.Bot.Modules.Crafting;
using MineSharp.Bot.Modules.Entity;
using MineSharp.Bot.Modules.Physics;
using MineSharp.Bot.Modules.Windows;
using MineSharp.Bot.Modules.World;
using MineSharp.Core;
using MineSharp.Core.Logging;
using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;
using MineSharp.Data.Blocks;
using MineSharp.Data.Protocol;
using MineSharp.MojangAuth;
using MineSharp.Protocol;
using MineSharp.Windows;
using System.Diagnostics;

namespace MineSharp.Bot
{
    public partial class MinecraftBot
    {
        private static readonly Logger Logger = Logger.GetLogger();
        
        public MinecraftClient Client { get; }
        public BotOptions Options { get; }
        public Session Session { get; }
        
        private readonly Dictionary<Type, List<Func<IPacketPayload, Task>>> PacketHandlers = new Dictionary<Type, List<Func<IPacketPayload, Task>>>();

        private readonly Dictionary<Type, object> packetWaiters = new Dictionary<Type, object>();
        
        public List<Module> Modules = new List<Module>();
        private readonly List<TickedModule> TickedModules = new List<TickedModule>();

        public readonly BaseModule? BaseModule;
        public readonly EntityModule? EntityModule;
        public readonly PhysicsModule? PhysicsModule;
        public readonly PlayerModule? PlayerModule;
        public readonly WindowsModule? WindowsModule;
        public readonly WorldModule? WorldModule;
        public readonly CraftingModule? CraftingModule;
        
        private Task? TickLoopTask;

        static MinecraftBot()
        {
            Logger.AddScope(LogLevel.DEBUG, s => Debug.WriteLine(s));
        }

        public MinecraftBot(BotOptions options)
        {
            this.Options = options;

            if (this.Options.Offline == true)
            {
                this.Session = Session.OfflineSession(this.Options.UsernameOrEmail);
            } else
            {
                this.Session = Session.Login().GetAwaiter().GetResult();
                Logger.Info("UUID: " + this.Session.UUID);
            }

            this.Client = new MinecraftClient(this.Options.Version, this.Session, this.Options.Host, this.Options.Port ?? 25565);
            this.Client.PacketReceived += this.Events_PacketReceived;

            this.BaseModule = new BaseModule(this);
            this.EntityModule = new EntityModule(this);
            this.PlayerModule = new PlayerModule(this);
            this.PhysicsModule = new PhysicsModule(this);
            this.WorldModule = new WorldModule(this);
            this.WindowsModule = new WindowsModule(this);
            this.CraftingModule = new CraftingModule(this);
        }

        public async Task LoadModule(Module module)
        {
            this.Modules.Add(module);
            if (module is TickedModule) this.TickedModules.Add((TickedModule)module);

            await module.Initialize();
            Logger.Info("Loaded module: " + module.GetType().Name);
        }

        public T? GetModule<T>() where T : Module
        {
            var module = this.Modules.FirstOrDefault(x => x.GetType() == typeof(T));
            return (T?)module;
        }

        private async Task TickLoop()
        {
            while (true)
            {
                var start = DateTime.Now;

                var tasks = new List<Task>();

                foreach (var tickedModule in this.TickedModules)
                {
                    if (tickedModule.IsEnabled)
                    {
                        tasks.Add(tickedModule.Tick());
                    }
                }
                await Task.WhenAll(tasks);

                var errors = tasks.Where(x => x.Exception != null);
                foreach (var err in errors)
                {
                    Logger.Error($"Error in Module: {err.Exception}");
                }

                var deltaTime = MinecraftConst.TickMs - (int)(DateTime.Now - start).TotalMilliseconds;
                if (deltaTime < 0)
                {
                    Logger.Warning($"Ticked modules taking too long, {-deltaTime}ms behind");
                } else
                    await Task.Delay(deltaTime);
            }
        }


        private async Task LoadModules()
        {
            await Task.WhenAll(this.LoadModule(this.BaseModule!), this.LoadModule(this.EntityModule!), this.LoadModule(this.PlayerModule!), this.LoadModule(this.PhysicsModule!), this.LoadModule(this.WorldModule!), this.LoadModule(this.WindowsModule!), this.LoadModule(this.CraftingModule!));
        }


        /// <summary>
        ///     Connects the bot to the server
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Connect()
        {
            if (!await this.Client.Connect(GameState.LOGIN))
            {
                return false;
            }

            await this.LoadModules();

            this.TickLoopTask = this.TickLoop();
            return true;
        }

        /// <summary>
        ///     Waits for a specific packet from the server
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [BotFunction("Basic", "Waits for a specific packet from the server")]
        public Task<T> WaitForPacket<T>() where T : IPacketPayload
        {
            var packetType = typeof(T);
            if (!this.packetWaiters.TryGetValue(packetType, out var task))
            {
                var tsc = new TaskCompletionSource<T>();
                this.packetWaiters.Add(packetType, tsc);

                return tsc.Task ?? throw new ArgumentNullException();
            }
            return ((TaskCompletionSource<T>?)task)?.Task ?? throw new ArgumentNullException();
        }

        [BotFunction("Basic", "Calls handler every time a specific packet is received")]
        public void On<T>(Func<T, Task> handler) where T : IPacketPayload
        {
            if (this.PacketHandlers.ContainsKey(typeof(T)))
            {
                this.PacketHandlers[typeof(T)].Add(p => handler((T)p));
            } else
            {
                this.PacketHandlers.Add(typeof(T), new List<Func<IPacketPayload, Task>> {
                    p => handler((T)p)
                });
            }
        }

        private void Events_PacketReceived(MinecraftClient client, IPacketPayload packet)
        {

            var packetType = packet.GetType();
            if (this.packetWaiters.TryGetValue(packetType, out var tsc))
            {
                if (null == typeof(TaskCompletionSource<>).MakeGenericType(packetType).GetMethod("TrySetResult")?.Invoke(tsc, new[] {
                        packet
                    }))
                {
                    throw new InvalidOperationException();
                }

                this.packetWaiters.Remove(packetType);
            }

            var tasks = new List<Task>();
            if (this.PacketHandlers.TryGetValue(packetType, out var handlers))
            {
                foreach (var handler in handlers)
                {
                    tasks.Add(handler(packet));
                }
            }

            Task.WaitAll(tasks.ToArray());
        }

        public struct BotOptions
        {
            public string Version;
            public string UsernameOrEmail;
            public bool? Offline;
            public string Host;
            public ushort? Port;
        }


        #region Events


        public delegate void BotEmptyEvent(MinecraftBot sender);
        public delegate void BotChatEvent(MinecraftBot sender, Chat chat);
        public delegate void BotChatMessageEvent(MinecraftBot sender, string message, MinecraftPlayer messageSender);
        public delegate void BotItemEvent(MinecraftBot sender, Item? item);

        public delegate void BotPlayerEvent(MinecraftBot sender, MinecraftPlayer entity);
        public delegate void BotEntityEvent(MinecraftBot sender, Entity entity);

        public delegate void BotWindowEvent(MinecraftBot sender, Window window);


        #endregion



        #region Public Methods


        #endregion

    }
}
