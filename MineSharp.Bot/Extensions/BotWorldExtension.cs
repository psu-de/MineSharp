using MineSharp.Bot.Enums;
using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;
using MineSharp.Data.Biomes;
using MineSharp.Data.Blocks;
using MineSharp.Physics;
using MineSharp.Protocol.Packets;
using MineSharp.Protocol.Packets.Serverbound.Play;
using MineSharp.World;
using MineSharp.World.Chunks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Bot {
    public partial class MinecraftBot {

        public World.World World => WorldModule.World;


        /// <summary>
        /// Returns the block at the given position
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Block GetBlockAt(Position pos) => World.GetBlockAt(pos);

        /// <summary>
        /// Returns the biome at the given position
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Biome GetBiomeAt(Position pos) => World.GetBiomeAt(pos);

        /// <summary>
        /// Searches through the loaded chunks for a specific block type 
        /// Can return less blocks than requested if not enough where found
        /// </summary>
        /// <param name="type">Block type to search for</param>
        /// <param name="count">Number of blocks to return. Use Count < 0 for no limit</param>
        /// <param name="cancellation">Optional to stop the searching task</param>
        /// <returns></returns>
        public Task<Block[]?> FindBlocksAsync(BlockType type, int count = -1, CancellationToken? cancellation = null) => World.FindBlocksAsync(type, count, cancellation);

        /// <summary>
        /// Searches through the loaded chunks for one block of a specific block type 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public Task<Block?> FindBlockAsync(BlockType type, CancellationToken? cancellation = null) => World.FindBlockAsync(type, cancellation);

        public Task<MineBlockStatus> MineBlock(Block block, BlockFace? face = null, CancellationToken? cancellation = null) => this.WorldModule.MineBlock(block, face, cancellation);

        public Task<Block?> Raycast (int length = 100) => this.WorldModule.Raycast(length);

        public Task WaitForChunksToLoad(int length = 5) => this.WorldModule.WaitForChunksToLoad(length);

    }
}
