using MineSharp.Bot;
using MineSharp.Bot.Enums;
using MineSharp.Core.Logging;
using MineSharp.Core.Types;

namespace MineSharp.Pathfinding.Moves
{
    public class DownMove : Move
    {
        private static readonly Logger Logger = Logger.GetLogger();
        public override Vector3 MoveVector { get; }

        private Vector3 _target;

        internal DownMove(Movements movements, Vector3 direction) : base(movements)
        {
            this.MoveVector = direction.Plus(Vector3.Down);
        }

        protected override Task Prepare(MinecraftBot bot)
        {
            this._target = bot.Player!.Entity.Position
                .Floored()
                .Plus(this.MoveVector)
                .Plus(new Vector3(0.5, 0, 0.5));
         
            Logger.Debug($"From {bot.Player!.Entity.Position} => {bot.Player!.Entity.Position.Floored()}");
            Logger.Debug($"DownMove: Target={this._target}");
            return Task.CompletedTask;
        }

        private const double THRESHOLD = 0.525d;

        protected override void OnTick(MinecraftBot sender)
        {
            var delta = sender.BotEntity!.Position.Minus(this._target);
            var deltaY = Math.Abs(delta.Y);
            delta.Y = 0;
            
            var length = delta.Length();

            if (length <= THRESHOLD)
            {
                if (deltaY <= 0.2d)
                {
                    this.TSC.SetResult();
                    return;
                }
            }
            
            Logger.Debug($"Distance to target: {delta.Length()}");
            Logger.Debug($"DeltaY: {deltaY}");

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
