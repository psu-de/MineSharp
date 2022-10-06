using MineSharp.Bot;
using MineSharp.Core.Types;

namespace MineSharp.Pathfinding.Moves
{
    public abstract class Move
    {
        protected Movements Movements { get; private set; }

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
        public async Task PerformMove(MinecraftBot bot)
        {
            this.TSC = new TaskCompletionSource();
            await this.Prepare(bot);
            
            bot.PhysicsTick += OnTick;
            await this.TSC.Task;
            bot.PhysicsTick -= OnTick;

            await this.Finish(bot);
        }

        protected abstract void OnTick(MinecraftBot sender);
    }
}
