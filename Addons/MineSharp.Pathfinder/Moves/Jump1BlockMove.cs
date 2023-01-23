using MineSharp.Bot;
using MineSharp.Bot.Enums;
using MineSharp.Core.Logging;
using MineSharp.Core.Types;
using MineSharp.Data;
using MineSharp.Data.Blocks;

namespace MineSharp.Pathfinding.Moves
{
    public class Jump1BlockMove : Move
    {
        private static readonly Logger Logger = Logger.GetLogger();
        public override float MoveCost => 50;
        public override Vector3 MoveVector { get; }

        public Jump1BlockMove(Movements movements, Vector3 direction) : base(movements)
        {
            this.MoveVector = direction * 2;
        }
        
        public override bool IsMovePossible(Vector3 startPosition, World.World world)
        {
            var target = startPosition.Plus(this.MoveVector);
            var blockToSkipPos = startPosition.Plus(this.MoveVector / 2);

            if (!this.HasBlockSpaceForStanding(target, world))
            {
                return false;
            }

            var blockToSkip = world.GetBlockAt(blockToSkipPos);
            if (blockToSkip.GetBoundingBoxes().Length > 0)
            {
                return false;
            }

            var blockToSkipAbove = world.GetBlockAt(blockToSkipPos.Plus(Vector3.Up));
            if (blockToSkipAbove.GetBoundingBoxes().Length > 0)
            {
                return false;
            }

            var blockToSkipAbove2 = world.GetBlockAt(blockToSkipPos.Plus(Vector3.Up * 2));
            if (blockToSkipAbove2.GetBoundingBoxes().Length > 0)
            {
                return false;
            }

            return true;
        }

        protected override void OnTick(MinecraftBot bot, Vector3 target)
        {
            var diff = bot.BotEntity!.Position.Minus(target);
            var length = diff.Length();
                
            var yaw = Math.Atan2(diff.X, -diff.Z) * (180 / Math.PI);
                
            bot.ForceSetRotation((float)yaw, 0);

            if (length is > 1d and < 1.5d)
            {
                bot.PlayerControls.Jump();
            }
            
            bot.PlayerControls.Walk(WalkDirection.Forward);

            if (length <= THRESHOLD)
            {
                this.TSC.SetResult();
                return;
            }
        }
    }
}
