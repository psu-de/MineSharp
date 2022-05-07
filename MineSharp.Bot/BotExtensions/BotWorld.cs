using MineSharp.Bot.Enums;
using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;
using MineSharp.Data.Blocks;
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
    public partial class Bot {

        public World.World World { get; private set; }

        private partial void LoadWorld() {
            this.World = new World.World();
        }

        #region Packet Handling

        private void handleChunkDataAndLightUpdate(MineSharp.Protocol.Packets.Clientbound.Play.ChunkDataAndLightUpdatePacket packet) {
            World.LoadChunkPacket(packet);
        }

        private void handleUnloadChunk(MineSharp.Protocol.Packets.Clientbound.Play.UnloadChunkPacket packet) {
            World.UnloadChunk(new ChunkCoordinates(packet.ChunkX, packet.ChunkZ));
        }

        private void handleBlockUpdate(MineSharp.Protocol.Packets.Clientbound.Play.BlockChangePacket packet) {
            Block newBlock = new Block(BlockData.StateToBlockMap[packet.BlockID], packet.Location, packet.BlockID);
            World.SetBlock(newBlock);
        }

        private void handleMultiBlockChange(MineSharp.Protocol.Packets.Clientbound.Play.MultiBlockChangePacket packet) {
            int sectionX = (int)(packet.Chunksectionposition >> 42);
            int sectionY = (int)(packet.Chunksectionposition & 0xFFFFF);
            int sectionZ = (int)((packet.Chunksectionposition >> 20) & 0x3FFFFF);

            if (sectionX > Math.Pow(2, 21)) sectionX -= (int)Math.Pow(2, 22);
            if (sectionY > Math.Pow(2, 19)) sectionY -= (int)Math.Pow(2, 20);
            if (sectionZ > Math.Pow(2, 21)) sectionZ -= (int)Math.Pow(2, 22);

            sectionY += Math.Abs(MineSharp.World.World.MinY / MineSharp.World.Chunks.Chunk.ChunkSectionLength);

            this.World.MultiblockUpdate(packet.Blocks, sectionX, sectionY, sectionZ);

        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Returns a Task that finishes once the Chunks in a square with length <paramref name="length"/> have been loaded
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public async Task WaitForChunksToLoad(int length = 5) {

            await WaitForBot();

            var playerChunk = World.GetChunkCoordinates((int)BotEntity.Position.X, (int)BotEntity.Position.Z);
            for (int x = playerChunk.X - length; x < playerChunk.X + length; x++) {
                for (int z = playerChunk.Z - length; z < playerChunk.Z + length; z++) {
                    while (World.GetChunkAt(new ChunkCoordinates(x, z)) == null) await Task.Delay(10);
                }
            }
        }

        /// <summary>
        /// Returns the block at the given position
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Block GetBlockAt(Position pos) => World.GetBlockAt(pos);

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
        /// <summary>
        /// Tries to mine the block with the currently held item
        /// </summary>
        /// <param name="block"></param>
        /// <param name="face"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public Task<MineBlockStatus> MineBlock(Block block, BlockFace? face = null, CancellationToken? cancellation = null) {
            return Task.Run(async () => {

                if (!block.Info.Diggable) return MineBlockStatus.NotDiggable;
                if (!World.IsBlockLoaded(block.Position)) return MineBlockStatus.BlockNotLoaded;


                if (face == null) {
                    // TODO: Maybe rausfinden wie des geht   
                }

                if (cancellation?.IsCancellationRequested ?? false) return MineBlockStatus.Cancelled;

                var packet = new MineSharp.Protocol.Packets.Serverbound.Play.PlayerDiggingPacket(DiggingStatus.StartedDigging, block.Position, face ?? BlockFace.Top);

                await this.Client.SendPacket(packet);

                int time = block.Info.CalculateBreakingTime(this.HeldItem?.Info, BotEntity);

                CancellationTokenSource cancelToken = new CancellationTokenSource();

                cancellation?.Register(() => cancelToken.Cancel());

                Task<Protocol.Packets.Clientbound.Play.AcknowledgePlayerDiggingPacket?> sendAgain = Task.Run(async () => {
                    await Task.Delay(time, cancelToken.Token);
                    if (cancelToken.Token.IsCancellationRequested) return null;

                    packet.Status = DiggingStatus.FinishedDigging;
                    await this.Client.SendPacket(packet);
                    return await this.WaitForPacket<Protocol.Packets.Clientbound.Play.AcknowledgePlayerDiggingPacket>() as Protocol.Packets.Clientbound.Play.AcknowledgePlayerDiggingPacket;
                });

                var ack = await this.WaitForPacket<Protocol.Packets.Clientbound.Play.AcknowledgePlayerDiggingPacket>() as Protocol.Packets.Clientbound.Play.AcknowledgePlayerDiggingPacket;
                if (cancellation?.IsCancellationRequested ?? false) {
                    cancelToken.Cancel();
                    await this.Client.SendPacket(new PlayerDiggingPacket(DiggingStatus.CancelledDigging, block.Position, face ?? BlockFace.Top));
                    return MineBlockStatus.Cancelled;
                }

                if (ack.Status != DiggingStatus.StartedDigging) {
                    return MineBlockStatus.Failed;
                }

                var secondPacket = await sendAgain;
                if (secondPacket == null || !secondPacket.Successful) {

                    return MineBlockStatus.Failed;
                }

                return MineBlockStatus.Finished;
            });
        }


        #endregion
    }
}
