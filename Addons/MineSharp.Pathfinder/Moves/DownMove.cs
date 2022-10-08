using MineSharp.Bot;
using MineSharp.Bot.Enums;
using MineSharp.Core.Logging;
using MineSharp.Core.Types;
using MineSharp.Data.Blocks;
using MineSharp.Physics;

namespace MineSharp.Pathfinding.Moves
{
    public class DownMove : Move
    {
        private static readonly Logger Logger = Logger.GetLogger();
        private const double THRESHOLD = 0.525d;
        public override Vector3 MoveVector { get; }

        private Vector3? _target;

        internal DownMove(Movements movements, Vector3 direction) : base(movements)
        {
            this.MoveVector = direction.Plus(Vector3.Down);
        }

        public override bool IsMovePossible(Vector3 startPosition, World.World world)
        {
            var target = startPosition.Plus(this.MoveVector);

            if (this.HasBlockSpaceForStanding(target, world))
            {
                var blockAbove2 = world.GetBlockAt(target.Plus(Vector3.Up * 2));
                var blockAbove2BBs = blockAbove2.GetBoundingBoxes();
                if (blockAbove2BBs.Length == 0 || !blockAbove2BBs.Any(x => Math.Truncate(x.MinY) > 0.8))
                {
                    return true;
                }
            }
            
            return false;
        }
        
        protected override Task Prepare(MinecraftBot bot)
        {
            this._target = bot.Player!.Entity.Position
                .Floored()
                .Plus(this.MoveVector)
                .Plus(new Vector3(0.5, 0, 0.5));
         
            Logger.Debug($"DownMove: Target={this._target}");
            return Task.CompletedTask;
        }


        protected override void OnTick(MinecraftBot sender)
        {
            var delta = sender.BotEntity!.Position.Minus(this._target!);
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
