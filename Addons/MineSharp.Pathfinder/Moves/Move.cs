using MineSharp.Bot;
using MineSharp.Core.Logging;
using MineSharp.Data.Blocks;
using MineSharp.Physics;
using MineSharp.Core.Types;
using MineSharp.Data;

namespace MineSharp.Pathfinding.Moves
{
    public abstract class Move
    {
        private static readonly Logger Logger = Logger.GetLogger();
        protected const double THRESHOLD = 0.25d * 0.25d;
        
        internal Move(Movements movements)
        {
            this.Movements = movements;
        }
        
        protected Movements Movements {
            get;
        }

        /// <summary>
        ///     Sets whether multiple moves of the same type in a path can be joined together
        /// </summary>
        internal virtual bool CanBeSimplified => false;

        /// <summary>
        ///     Cost for A* Algorithm. With a higher cost, the move will be performed less likely.
        /// </summary>
        public abstract float MoveCost { get; }

        /// <summary>
        ///     The relative position after the mvoe
        /// </summary>
        public abstract Vector3 MoveVector { get; }

        /// <summary>
        ///     TSC should be set in the OnTick method when the bot has completed the move
        /// </summary>
        protected TaskCompletionSource TSC { get; private set; }

        /// <summary>
        ///     Checks whether the move is possible from a given start position
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="world"></param>
        /// <returns></returns>
        public abstract bool IsMovePossible(Vector3 startPosition, World.World world);
        
        /// <summary>
        ///     Perform the move
        /// </summary>
        /// <param name="bot">The <see cref="MinecraftBot" /> that should perform the move</param>
        /// <returns></returns>
        public async Task PerformMove(MinecraftBot bot, Vector3 startPosition, int count, CancellationToken? cancellation = null)
        {
            if (count <= 0)
            {
                throw new ArgumentException("Count must be greater than 0");
            }
            
            if (!CanBeSimplified && count != 1)
            {
                throw new ArgumentException("Move cannot be performed multiple times");
            }
            
            cancellation ??= CancellationToken.None;
            this.TSC = new TaskCompletionSource();

            var target = startPosition
                .Plus(this.MoveVector * count)
                .Plus(new Vector3(0.5d, 0, 0.5d));
            
            Logger.Debug($"{this.GetType().Name}: From={startPosition} Target={target} Count={count}");

            MinecraftBot.BotEmptyEvent onTick = (MinecraftBot sender) =>
            {
                OnTick(sender, target);
            };
            
            bot.PhysicsTick += onTick;
            await this.TSC.Task.WaitAsync(cancellation.Value);
            bot.PhysicsTick -= onTick;
        }

        protected abstract void OnTick(MinecraftBot bot, Vector3 target);

        protected bool HasBlockSpaceForStanding(Vector3 pos, World.World world)
        {
            var playerBB = PhysicsConst.GetPlayerBoundingBox(pos).Offset(0.5d, 0,0.5d);

            var targetBlock = world.GetBlockAt(pos.Floored());
            var targetBBs = targetBlock.GetBoundingBoxes();
            if (targetBBs.Length == 0 || !targetBBs.Any(x => playerBB.Intersects(x)))
            {
                var blockBelow = world.GetBlockAt(pos.Plus(Vector3.Down));
                var blockBelowBBs = blockBelow.GetBoundingBoxes();
                if (blockBelowBBs.Length > 0 && blockBelowBBs.All(x => playerBB.MinY - x.MaxY is >= 0 and < 0.2))
                {
                    var blockAbove = world.GetBlockAt(pos.Plus(Vector3.Up));
                    var blockAboveBBs = blockAbove.GetBoundingBoxes();
                    if (blockAboveBBs.Length == 0 || blockAboveBBs.All(x => x.MinY >= playerBB.MaxY)) // TODO: Sneaking
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
