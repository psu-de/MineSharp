using MineSharp.Core.Types;
using MineSharp.Bot;
using MineSharp.Bot.Enums;
using MineSharp.Core.Logging;
using MineSharp.Data.Blocks;
using MineSharp.Physics;

namespace MineSharp.Pathfinding.Moves
{
    public class DirectMove : Move
    {
        private static readonly Logger Logger = Logger.GetLogger();

        public override float MoveCost => 0;

        public override Vector3 MoveVector { get; }
        private const double THRESHOLD = 0.0525d;

        private Vector3? _target;
        
        internal DirectMove(Movements movements, Vector3 direction) : base(movements)
        {
            this.MoveVector = direction;
        }

        public override bool IsMovePossible(Vector3 startPosition, World.World world)
        {
            var target = startPosition.Plus(this.MoveVector);
            return this.HasBlockSpaceForStanding(target, world);
        }

        protected override Task Prepare(MinecraftBot bot)
        {
            this._target = bot.BotEntity!.Position.Floored()
                .Plus(this.MoveVector)
                .Plus(new Vector3(0.5d, 0, 0.5d));
            Logger.Debug($"DirectMove: Target={this._target!}");
            return Task.CompletedTask;
        }

        protected override Task Finish(MinecraftBot bot)
        {
            return bot.PlayerControls.Reset();
        }
        
        protected override void OnTick (MinecraftBot sender)
        {
            var delta = sender.BotEntity!.Position.Minus(this._target!);
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
