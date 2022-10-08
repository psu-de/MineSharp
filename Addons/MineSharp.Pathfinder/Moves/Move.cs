using MineSharp.Bot;
using MineSharp.Core.Types;
using MineSharp.Data.Blocks;
using MineSharp.Physics;

namespace MineSharp.Pathfinding.Moves
{
    public abstract class Move
    {
        protected Movements Movements { get; private set; }

        /// <summary>
        /// Cost for A* Algorithm. With a higher cost, the move will be performed less likely.
        /// </summary>
        public abstract float MoveCost { get; }

        /// <summary>
        /// The relative position after the mvoe
        /// </summary>
        public abstract Vector3 MoveVector { get; }
        
        /// <summary>
        /// TSC should be set in the OnTick method when the bot has completed the move
        /// </summary>
        protected TaskCompletionSource TSC { get; private set; }

        internal Move(Movements movements)
        {
            Movements = movements;
        }

        /// <summary>
        /// Checks whether the move is possible from a given start position
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="world"></param>
        /// <returns></returns>
        public abstract bool IsMovePossible(Vector3 startPosition, World.World world);

        /// <summary>
        /// Called just before performing the move
        /// </summary>
        /// <param name="bot"></param>
        protected virtual Task Prepare(MinecraftBot bot)
        {
            return Task.CompletedTask; 
        }

        /// <summary>
        /// Called right after the move has been completed
        /// </summary>
        /// <param name="bot"></param>
        protected virtual Task Finish(MinecraftBot bot)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Perform the move
        /// </summary>
        /// <param name="bot">The <see cref="MinecraftBot"/> that should perform the move</param>
        /// <returns></returns>
        public async Task PerformMove(MinecraftBot bot, CancellationToken? cancellation = null)
        {
            cancellation = cancellation ?? CancellationToken.None;
            this.TSC = new TaskCompletionSource();
            await this.Prepare(bot);
            
            bot.PhysicsTick += OnTick;
            await this.TSC.Task.WaitAsync(cancellation.Value);
            bot.PhysicsTick -= OnTick;

            await this.Finish(bot);
        }

        protected abstract void OnTick(MinecraftBot sender);

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
