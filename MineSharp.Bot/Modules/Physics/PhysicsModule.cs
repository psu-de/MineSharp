using MineSharp.Core.Types;
using MineSharp.Data.Protocol.Play.Serverbound;
using MineSharp.Physics;
using static MineSharp.Bot.MinecraftBot;

namespace MineSharp.Bot.Modules.Physics
{
    public class PhysicsModule : TickedModule
    {

        private const double POSITION_THRESHOLD = 0.2d;

        private PlayerInfoState LastPlayerState;

        public PhysicsEngine? Physics;

        public PlayerControls PlayerControls;

        public PhysicsModule(MinecraftBot bot) : base(bot)
        {
            this.PlayerControls = new PlayerControls(bot);
        }

        /// <summary>
        ///     Fires when the Bot <see cref="BotEntity" /> moves
        /// </summary>
        public event BotPlayerEvent? BotMoved;

        /// <summary>
        ///     Fires just before executing a physics tick
        /// </summary>
        public event BotEmptyEvent? PhysicsTick;

        protected override async Task Load()
        {
            await this.Bot.WaitForBot();
            await this.Bot.WaitForChunksToLoad();

            await this.UpdateServerPos();

            this.Physics = new PhysicsEngine(this.Bot.BotEntity!, this.Bot.World!);
            await this.SetEnabled(true);
        }

        public override Task Tick()
        {
            this.PhysicsTick?.Invoke(this.Bot);
            return Task.Run(async () =>
            {
                try
                {
                    this.Physics!.SimulatePlayer(this.PlayerControls.PrepareForPhysicsTick());
                    await this.UpdateServerPositionIfNeeded();
                } catch (Exception e)
                {
                    this.Logger.Error(e.ToString());
                }
            });
        }


        private async Task UpdateServerPositionIfNeeded()
        {
            if (this.LastPlayerState.X != this.Bot.BotEntity!.Position.X || this.LastPlayerState.Y != this.Bot.BotEntity.Position.Y || this.LastPlayerState.Z != this.Bot.BotEntity.Position.Z || this.LastPlayerState.Yaw != this.Bot.BotEntity.Yaw || this.LastPlayerState.Pitch != this.Bot.BotEntity.Pitch || this.LastPlayerState.IsOnGround != this.Bot.BotEntity.IsOnGround)
            {
                await this.UpdateServerPos();
            }
        }

        private async Task UpdateServerPos()
        {
            var packet = new PacketPositionLook(this.Bot.BotEntity!.Position.X, this.Bot.BotEntity.Position.Y, this.Bot.BotEntity.Position.Z, this.Bot.BotEntity.Yaw, this.Bot.BotEntity.Pitch, this.Bot.BotEntity.IsOnGround);


            this.LastPlayerState.X = this.Bot.BotEntity.Position.X;
            this.LastPlayerState.Y = this.Bot.BotEntity.Position.Y;
            this.LastPlayerState.Z = this.Bot.BotEntity.Position.Z;
            this.LastPlayerState.Yaw = this.Bot.BotEntity.Yaw;
            this.LastPlayerState.Pitch = this.Bot.BotEntity.Pitch;
            this.LastPlayerState.IsOnGround = this.Bot.BotEntity.IsOnGround;

            await this.Bot.Client.SendPacket(packet);
            this.BotMoved?.Invoke(this.Bot, this.Bot.Player!);
        }

        /// <summary>
        ///     Forces the bots rotation to the given yaw and pitch (in degrees)
        /// </summary>
        /// <param name="yaw"></param>
        /// <param name="pitch"></param>
        public void ForceSetRotation(float yaw, float pitch)
        {
            this.Bot.BotEntity!.Yaw = yaw;
            this.Bot.BotEntity!.Pitch = pitch;
        }

        /// <summary>
        ///     Forces the bot to look at given position
        /// </summary>
        /// <param name="position"></param>
        public void ForceLookAt(Position position)
        {

            var pos = ((Vector3)position).Plus(new Vector3(0.5d, 0.5d, 0.5d));
            var r = pos.Minus(this.Bot.Player!.GetHeadPosition());
            var yaw = -Math.Atan2(r.X, r.Z) / Math.PI * 180;
            if (yaw < 0) yaw = 360 + yaw;
            var pitch = -Math.Asin(r.Y / r.Length()) / Math.PI * 180;
            this.ForceSetRotation((float)yaw, (float)pitch);
        }

        private struct PlayerInfoState
        {
            public double X;
            public double Y;
            public double Z;
            public float Yaw;
            public float Pitch;
            public bool IsOnGround;

            public override string ToString() => $"X={this.X} Y={this.Y} Z={this.Z} Yaw={this.Yaw} Pitch={this.Pitch} IsOnGround={this.IsOnGround}";
        }
    }
}
