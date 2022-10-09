using MineSharp.Bot;
using MineSharp.Bot.Enums;
using MineSharp.Core.Logging;
using MineSharp.Core.Types;
using MineSharp.Data.Blocks;

namespace MineSharp.Pathfinding.Moves
{
    public class DownMove : Move
    {
        private const double THRESHOLD = 0.525d;
        private static readonly Logger Logger = Logger.GetLogger();

        internal DownMove(Movements movements, Vector3 direction) : base(movements)
        {
            this.MoveVector = direction.Plus(Vector3.Down);
        }

        public override float MoveCost => 15;

        public override Vector3 MoveVector { get; }

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

        protected override void OnTick(MinecraftBot bot, Vector3 target)
        {
            var delta = bot.BotEntity!.Position.Minus(target!);
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
            bot.ForceSetRotation((float)yaw, 0);

            bot.PlayerControls.Walk(WalkDirection.Forward);
            if (this.Movements.AllowSprinting)
            {
                _ = bot.PlayerControls.StartSprinting();
            }  
        }
    }
}
