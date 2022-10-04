using MineSharp.Bot.Modules.Physics;
using MineSharp.Core.Types;
using MineSharp.Physics;
using static MineSharp.Bot.MinecraftBot;

namespace MineSharp.Bot.Modules {
    public class PhysicsModule : TickedModule {

        public PhysicsEngine? Physics;

        public PlayerControls PlayerControls;
        private PlayerInfoState LastPlayerState = new PlayerInfoState();

        private List<(Vector3, TaskCompletionSource)> AwaitedPositions = new List<(Vector3, TaskCompletionSource)>();
        
        /// <summary>
        /// Fires when the Bot <see cref="BotEntity"/> moves
        /// </summary>
        public event BotPlayerEvent? BotMoved;

        /// <summary>
        /// Fires just before executing a physics tick
        /// </summary>
        public event BotEmptyEvent? PhysicsTick;

        private struct PlayerInfoState {
            public double X;
            public double Y;
            public double Z;
            public float Yaw;
            public float Pitch;
            public bool IsOnGround;

            public override string ToString() {
                return $"X={X} Y={Y} Z={Z} Yaw={Yaw} Pitch={Pitch} IsOnGround={IsOnGround}";
            }
        }

        public PhysicsModule(MinecraftBot bot) : base(bot) {
            PlayerControls = new PlayerControls(bot);
            BotMoved += handleBotMoved;
        }

        protected override async Task Load() {
            await this.Bot.WaitForBot();
            await this.Bot.WaitForChunksToLoad();

            await UpdateServerPos();

            this.Physics = new PhysicsEngine(Bot.BotEntity!, Bot.World!);
            await this.SetEnabled(true);
        }

        public override Task Tick() {
            this.PhysicsTick?.Invoke(this.Bot);
            return Task.Run(async () => {
                try
                {
                    this.Physics!.SimulatePlayer(this.PlayerControls.PrepareForPhysicsTick());
                    await UpdateServerPositionIfNeeded();
                } catch (Exception e)
                {
                    Logger.Error(e.ToString());
                }
            });
        }


        private async Task UpdateServerPositionIfNeeded () {
            if (LastPlayerState.X != Bot.BotEntity!.Position.X ||
                LastPlayerState.Y != Bot.BotEntity.Position.Y ||
                LastPlayerState.Z != Bot.BotEntity.Position.Z ||
                LastPlayerState.Yaw != Bot.BotEntity.Yaw || 
                LastPlayerState.Pitch != Bot.BotEntity.Pitch ||
                LastPlayerState.IsOnGround != Bot.BotEntity.IsOnGround) {
                await UpdateServerPos();
            }
        }

        private async Task UpdateServerPos() {
            var packet = new Data.Protocol.Play.Serverbound.PacketPositionLook(
               Bot.BotEntity!.Position.X, Bot.BotEntity.Position.Y, Bot.BotEntity.Position.Z, 
               Bot.BotEntity.Yaw, Bot.BotEntity.Pitch,
               Bot.BotEntity.IsOnGround);


            LastPlayerState.X = Bot.BotEntity.Position.X;
            LastPlayerState.Y = Bot.BotEntity.Position.Y;
            LastPlayerState.Z = Bot.BotEntity.Position.Z;
            LastPlayerState.Yaw = Bot.BotEntity.Yaw;
            LastPlayerState.Pitch = Bot.BotEntity.Pitch;
            LastPlayerState.IsOnGround = Bot.BotEntity.IsOnGround;

            await Bot.Client.SendPacket(packet);
            BotMoved?.Invoke(this.Bot, this.Bot.Player!);
        }
        
        /// <summary>
        /// Forces the bots rotation to the given yaw and pitch (in degrees)
        /// </summary>
        /// <param name="yaw"></param>
        /// <param name="pitch"></param>
        public void ForceSetRotation(float yaw, float pitch) {
            this.Bot.BotEntity!.Yaw = yaw;
            this.Bot.BotEntity!.Pitch = pitch;
        }

        /// <summary>
        /// Forces the bot to look at given position
        /// </summary>
        /// <param name="position"></param>
        public void ForceLookAt(Position position) {

            var pos = ((Vector3)position).Plus(new Vector3(0.5d, 0.5d, 0.5d));
            var r = pos.Minus(this.Bot.Player!.GetHeadPosition());
            double yaw = -Math.Atan2(r.X, r.Z) / Math.PI * 180;
            if (yaw < 0) yaw = 360 + yaw;
            double pitch = -Math.Asin(r.Y / r.Length()) / Math.PI * 180;
            ForceSetRotation((float)yaw, (float)pitch);
        }

        private const double POSITION_THRESHOLD = 0.2d; 
        private void handleBotMoved(MinecraftBot sender, MinecraftPlayer entity)
        {
            foreach ((var pos, var tsc) in AwaitedPositions.ToArray())
            {
                if (Math.Truncate(pos.Y) == Math.Truncate(entity.Entity.Position.Y))
                {
                    var dx = pos.X - entity.Entity.Position.X;
                    var dz = pos.Z - entity.Entity.Position.Z;
                    var length = dx * dx + dz * dz;
                    
                    if (dx * dx + dz * dz < POSITION_THRESHOLD * POSITION_THRESHOLD)
                    {
                        tsc.SetResult();
                        AwaitedPositions.Remove((pos, tsc));
                    }
                }
            }
        }
        
        public Task WaitUntilReached(Vector3 pos)
        {
            var tsc = new TaskCompletionSource();
            AwaitedPositions.Add((pos, tsc));
            return tsc.Task;
        }
    }
}
