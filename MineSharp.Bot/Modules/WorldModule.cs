using MineSharp.Bot.Enums;
using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;
using MineSharp.Data.Blocks;
using MineSharp.Protocol.Packets.Clientbound.Play;
using MineSharp.Protocol.Packets.Serverbound.Play;
using MineSharp.World.Chunks;

namespace MineSharp.Bot.Modules {
    public class WorldModule : Module {


        public World.World World { get; private set; }

        public WorldModule(Bot.MinecraftBot bot) : base(bot) { }

        protected override Task Load() {

            World = new World.World();

            Bot.On<ChunkDataAndLightUpdatePacket>(handleChunkDataAndLightUpdate);
            Bot.On<UnloadChunkPacket>(handleUnloadChunk);
            Bot.On<BlockChangePacket>(handleBlockUpdate);
            Bot.On<MultiBlockChangePacket>(handleMultiBlockChange);

            return Task.CompletedTask;
        }


        private Task handleChunkDataAndLightUpdate(MineSharp.Protocol.Packets.Clientbound.Play.ChunkDataAndLightUpdatePacket packet) {
            World.LoadChunkPacket(packet);
            return Task.CompletedTask;
        }

        private Task handleUnloadChunk(MineSharp.Protocol.Packets.Clientbound.Play.UnloadChunkPacket packet) {
            World.UnloadChunk(new ChunkCoordinates(packet.ChunkX, packet.ChunkZ));
            return Task.CompletedTask;
        }

        private Task handleBlockUpdate(MineSharp.Protocol.Packets.Clientbound.Play.BlockChangePacket packet) {

            var blockId = BlockPalette.GetBlockIdByState(packet.BlockID);

            Block newBlock = BlockFactory.CreateBlock(blockId, packet.BlockID, packet.Location!);
            World.SetBlock(newBlock);
            return Task.CompletedTask;
        }

        private Task handleMultiBlockChange(MineSharp.Protocol.Packets.Clientbound.Play.MultiBlockChangePacket packet) {
            int sectionX = (int)(packet.Chunksectionposition >> 42);
            int sectionY = (int)(packet.Chunksectionposition & 0xFFFFF);
            int sectionZ = (int)((packet.Chunksectionposition >> 20) & 0x3FFFFF);

            if (sectionX > Math.Pow(2, 21)) sectionX -= (int)Math.Pow(2, 22);
            if (sectionY > Math.Pow(2, 19)) sectionY -= (int)Math.Pow(2, 20);
            if (sectionZ > Math.Pow(2, 21)) sectionZ -= (int)Math.Pow(2, 22);

            sectionY += Math.Abs(MineSharp.World.World.MinY / MineSharp.World.Chunks.Chunk.ChunkSectionLength);

            this.World.MultiblockUpdate(packet.Blocks, sectionX, sectionY, sectionZ);
            return Task.CompletedTask;
        }



        public Task<Block?> Raycast(int length = 100) {
            return Task.Run(() => {

                var vc = this.Bot.BotEntity!.GetDirectionVector() / 10; // TODO: Vector kann block überspringen
                var lc = this.Bot.Player!.GetHeadPosition().Clone();

                for (int i = 0; i < 10 * length; i++) {
                    try {
                        var b = this.World.GetBlockAt(lc.Floored());
                        if (b.IsSolid()) {
                            List<AABB> boundingBoxes = new List<AABB>();
                            foreach (BlockShape shape in b.GetBlockShape()) {
                                var bb = shape.ToBoundingBox();
                                bb.Offset(b.Position!.X, b.Position.Y, b.Position.Z);
                                if (bb.Contains(lc.X, lc.Y, lc.Z))
                                    return b;
                            }
                        }
                    } catch (ArgumentException) {
                        return null;
                    }
                    lc.Add(vc);
                }
                return null;
            });
        }

        /// <summary>
        /// Returns a Task that finishes once the Chunks in a square with length <paramref name="length"/> have been loaded
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public async Task WaitForChunksToLoad(int length = 5) {

            await Bot.WaitForBot();

            var playerChunk = World.GetChunkCoordinates((int)this.Bot.BotEntity.Position.X, (int)this.Bot.BotEntity.Position.Z);
            for (int x = playerChunk.X - length; x < playerChunk.X + length; x++) {
                for (int z = playerChunk.Z - length; z < playerChunk.Z + length; z++) {
                    while (World.GetChunkAt(new ChunkCoordinates(x, z)) == null) await Task.Delay(10);
                }
            }
        }

        /// <summary>
        /// Tries to mine the block with the currently held item
        /// </summary>
        /// <param name="block"></param>
        /// <param name="face"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public Task<MineBlockStatus> MineBlock(Block block, BlockFace? face = null, CancellationToken? cancellation = null) {
            return Task.Run(async () => {

                if (!block.Diggable) return MineBlockStatus.NotDiggable;
                if (!World.IsBlockLoaded(block.Position!)) return MineBlockStatus.BlockNotLoaded;


                if (face == null) {
                    // TODO: Maybe rausfinden wie des geht   
                }

                if (cancellation?.IsCancellationRequested ?? false) return MineBlockStatus.Cancelled;

                var packet = new PlayerDiggingPacket(DiggingStatus.StartedDigging, block.Position, face ?? BlockFace.Top);

                await this.Bot.Client.SendPacket(packet);

                int time = block.CalculateBreakingTime(this.Bot.HeldItem, this.Bot.BotEntity);

                CancellationTokenSource cancelToken = new CancellationTokenSource();

                cancellation?.Register(() => cancelToken.Cancel());

                Task<AcknowledgePlayerDiggingPacket?> sendAgain = Task.Run(async () => {
                    await Task.Delay(time, cancelToken.Token);
                    if (cancelToken.Token.IsCancellationRequested) return null;

                    packet.Status = DiggingStatus.FinishedDigging;
                    await this.Bot.Client.SendPacket(packet);
                    return await Bot.WaitForPacket<AcknowledgePlayerDiggingPacket>();
                });

                var ack = await Bot.WaitForPacket<AcknowledgePlayerDiggingPacket>();
                if (cancellation?.IsCancellationRequested ?? false) {
                    cancelToken.Cancel();
                    await this.Bot.Client.SendPacket(new PlayerDiggingPacket(DiggingStatus.CancelledDigging, block.Position, face ?? BlockFace.Top));
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
    }
}
