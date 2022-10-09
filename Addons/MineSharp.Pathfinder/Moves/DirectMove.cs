using MineSharp.Bot;
using MineSharp.Bot.Enums;
using MineSharp.Core.Logging;
using MineSharp.Core.Types;

namespace MineSharp.Pathfinding.Moves
{
    public class DirectMove : Move
    {
        private static readonly Logger Logger = Logger.GetLogger();
        
        internal DirectMove(Movements movements, Vector3 direction) : base(movements)
        {
            this.MoveVector = direction;
        }

        public override float MoveCost => 0;

        public override Vector3 MoveVector { get; }

        internal override bool CanBeSimplified => true;

        public override bool IsMovePossible(Vector3 startPosition, World.World world)
        {
            var target = startPosition.Plus(this.MoveVector);
            var possible = this.HasBlockSpaceForStanding(target, world);
            return possible;
        }

        protected override void OnTick(MinecraftBot bot, Vector3 target)
        {
            var delta = bot.BotEntity!.Position.Minus(target);
            var length = delta.Length();
                
            if (length <= THRESHOLD)
            {
                Logger.Debug("Reached target");
                this.TSC.SetResult();
                return;
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
