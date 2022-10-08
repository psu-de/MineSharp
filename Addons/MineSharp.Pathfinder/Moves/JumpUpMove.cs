using MineSharp.Bot;
using MineSharp.Bot.Enums;
using MineSharp.Core.Logging;
using MineSharp.Core.Types;
using MineSharp.Data.Blocks;

namespace MineSharp.Pathfinding.Moves
{
    public class JumpUpMove : Move
    {
        private const double THRESHOLD = 0.525d;
        private static readonly Logger Logger = Logger.GetLogger();

        private Vector3 _target;

        internal JumpUpMove(Movements movements, Vector3 direction) : base(movements)
        {
            this.MoveVector = direction.Plus(Vector3.Up);
        }

        public override float MoveCost => 5;

        public override Vector3 MoveVector { get; }

        public override bool IsMovePossible(Vector3 startPosition, World.World world)
        {
            var target = startPosition.Plus(this.MoveVector);

            if (this.HasBlockSpaceForStanding(target, world))
            {
                var blockAbove2 = world.GetBlockAt(startPosition.Plus(Vector3.Up * 2));
                var blockAbove2BBs = blockAbove2.GetBoundingBoxes();
                if (blockAbove2BBs.Length == 0)
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

            Logger.Debug($"JumpUpMove: Target={this._target}");
            return Task.CompletedTask;
        }


        protected override void OnTick(MinecraftBot sender)
        {
            var delta = sender.BotEntity!.Position.Minus(this._target);
            var deltaY = Math.Abs(delta.Y);
            delta.Y = 0;

            if (this.Movements.AllowSprinting)
            {
                _ = sender.PlayerControls.StartSprinting();
            }

            var yaw = Math.Atan2(delta.X, -delta.Z) * (180 / Math.PI);
            sender.ForceSetRotation((float)yaw, 0);

            sender.PlayerControls.Walk(WalkDirection.Forward);

            if (deltaY > 0.6)
            {
                sender.PlayerControls.Jump();
            }

            if (deltaY < 0.2 && delta.Length() <= THRESHOLD)
            {
                this.TSC.SetResult();
            }
        }
    }
}
