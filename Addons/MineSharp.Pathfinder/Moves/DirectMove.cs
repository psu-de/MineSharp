using MineSharp.Core.Types;
using MineSharp.Bot;
using MineSharp.Bot.Enums;
using MineSharp.Core.Logging;

namespace MineSharp.Pathfinding.Moves
{
    public class DirectMove : Move
    {
        private static readonly Logger Logger = Logger.GetLogger();
        private Vector3 _direction;
        public override Vector3 MoveVector => _direction;
        
        private Vector3 _target;
        
        internal DirectMove(Movements movements, Vector3 direction) : base(movements)
        {
            this._direction = direction;
        }

        protected override Task Prepare(MinecraftBot bot)
        {
            this._target = bot.BotEntity!.Position.Floored()
                .Plus(this.MoveVector)
                .Plus(new Vector3(0.5d, 0, 0.5d));
            Logger.Debug($"DirectMove: Target={this._target}");
            return Task.CompletedTask;
        }

        protected override Task Finish(MinecraftBot bot)
        {
            return bot.PlayerControls.Reset();
        }

        private const double THRESHOLD = 0.0525d;
        
        protected override void OnTick (MinecraftBot sender)
        {
            var delta = sender.BotEntity!.Position.Minus(this._target);
            var length = delta.Length();

            if (length <= THRESHOLD)
            {
                this.TSC.SetResult();
                return;
            }

            var yaw = Math.Atan2(delta.X, -delta.Z) * (180 / Math.PI);
            sender.ForceSetRotation((float)yaw, 0);

            sender.PlayerControls.Walk(WalkDirection.Forward);
            if (Movements.AllowSprinting)
            {
                _ = sender.PlayerControls.StartSprinting();
            }
        }
    }
}
