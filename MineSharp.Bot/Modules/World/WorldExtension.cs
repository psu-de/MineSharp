using MineSharp.Bot.Enums;
using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;
using MineSharp.Data.Blocks;

namespace MineSharp.Bot
{
    public partial class MinecraftBot
    {

        public World.World? World => this.WorldModule!.World;


        /// <summary>
        ///     Returns the block at the given position
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        [BotFunction("World", "Gets the block at the given position")]
        public Block GetBlockAt(Position pos) => this.World!.GetBlockAt(pos);

        /// <summary>
        ///     Returns the biome at the given position
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        [BotFunction("World", "Gets the biome at the given position")]
        public Biome GetBiomeAt(Position pos) => this.World!.GetBiomeAt(pos);

        /// <summary>
        ///     Searches through the loaded chunks for a specific block type
        ///     Can return less blocks than requested if not enough where found
        /// </summary>
        /// <param name="type">Block type to search for</param>
        /// <param name="count">Number of blocks to return. Use Count < 0 for no limit</param>
        /// <param name="cancellation">Optional to stop the searching task</param>
        /// <returns></returns>
        [BotFunction("World", "Finds a number of blocks of a given block type")]
        public Task<Block[]?> FindBlocksAsync(BlockType type, int count = -1, CancellationToken? cancellation = null) => this.World!.FindBlocksAsync(type, count, cancellation);

        /// <summary>
        ///     Searches through the loaded chunks for one block of a specific block type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        [BotFunction("World", "Finds a block of the given block type")]
        public Task<Block?> FindBlockAsync(BlockType type, CancellationToken? cancellation = null) => this.World!.FindBlockAsync(type, cancellation);


        [BotFunction("World", "Mines the block")]
        public Task<MineBlockStatus> MineBlock(Block block, BlockFace? face = null, CancellationToken? cancellation = null) => this.WorldModule!.MineBlock(block, face, cancellation);


        [BotFunction("World", "Returns the block the bot is looking at")]
        public Task<Block?> Raycast(int length = 100) => this.WorldModule!.Raycast(length);


        [BotFunction("World", "Waits until the chunks around the bot have been loaded")]
        public Task WaitForChunksToLoad(int length = 5) => this.WorldModule!.WaitForChunksToLoad(length);
    }
}
