using MineSharp.Bot;
using MineSharp.Data.Blocks;
using MineSharp.Physics;
using System.Numerics;
using Vector3 = MineSharp.Core.Types.Vector3;

namespace MineSharp.Pathfinding.Moves
{
    public abstract class Move
    {
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
        ///     Called just before performing the move
        /// </summary>
        /// <param name="bot"></param>
        protected virtual Task Prepare(MinecraftBot bot, int count, Vector3 startPosition) => Task.CompletedTask;

        /// <summary>
        ///     Called right after the move has been completed
        /// </summary>
        /// <param name="bot"></param>
        protected virtual Task Finish(MinecraftBot bot) => Task.CompletedTask;

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
            
            cancellation = cancellation ?? CancellationToken.None;
            this.TSC = new TaskCompletionSource();
            await this.Prepare(bot, count, startPosition);

            var onTick = this.OnTickWrapper();
            
            bot.PhysicsTick += onTick;
            await this.TSC.Task.WaitAsync(cancellation.Value);
            bot.PhysicsTick -= onTick;

            await this.Finish(bot);
        }

        protected abstract MinecraftBot.BotEmptyEvent OnTickWrapper();

        protected bool HasBlockSpaceForStanding(Vector3 pos, World.World world)
        {
            var playerBB = PhysicsConst.GetPlayerBoundingBox(pos);

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
