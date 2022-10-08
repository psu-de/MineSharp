using MineSharp.Bot.Enums;
using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;
using MineSharp.Data;
using MineSharp.Data.Blocks;
using MineSharp.Data.Protocol.Play.Clientbound;
using MineSharp.Data.Protocol.Play.Serverbound;
using MineSharp.World.Chunks;

namespace MineSharp.Bot.Modules
{
    public class WorldModule : Module
    {

        public WorldModule(MinecraftBot bot) : base(bot) {}


        public World.World? World { get; private set; }

        protected override Task Load()
        {

            this.World = new World.World();

            this.Bot.On<PacketMapChunk>(this.handleChunkDataAndLightUpdate);
            this.Bot.On<PacketUnloadChunk>(this.handleUnloadChunk);
            this.Bot.On<PacketBlockChange>(this.handleBlockUpdate);
            this.Bot.On<PacketMultiBlockChange>(this.handleMultiBlockChange);

            return Task.CompletedTask;
        }


        private Task handleChunkDataAndLightUpdate(PacketMapChunk packet)
        {
            this.World!.LoadChunkPacket(packet);
            return Task.CompletedTask;
        }

        private Task handleUnloadChunk(PacketUnloadChunk packet)
        {
            this.World!.UnloadChunk(new ChunkCoordinates(packet.ChunkX, packet.ChunkZ));
            return Task.CompletedTask;
        }

        private Task handleBlockUpdate(PacketBlockChange packet)
        {

            var blockId = BlockPalette.GetBlockIdByState(packet.Type!);

            var newBlock = BlockFactory.CreateBlock(blockId, packet.Type!, new Position(packet.Location.Value));
            this.World!.SetBlock(newBlock);
            return Task.CompletedTask;
        }

        private Task handleMultiBlockChange(PacketMultiBlockChange packet)
        {
            var sectionX = packet.ChunkCoordinates.X;
            var sectionY = packet.ChunkCoordinates.Y;
            var sectionZ = packet.ChunkCoordinates.Z;

            if (sectionX > Math.Pow(2, 21)) sectionX -= (int)Math.Pow(2, 22);
            if (sectionY > Math.Pow(2, 19)) sectionY -= (int)Math.Pow(2, 20);
            if (sectionZ > Math.Pow(2, 21)) sectionZ -= (int)Math.Pow(2, 22);

            sectionY += Math.Abs(MineSharp.World.World.MinY / Chunk.ChunkSectionLength);

            this.World!.MultiblockUpdate(packet.Records!.Select(x => x.Value).ToArray(), sectionX, sectionY, sectionZ);
            return Task.CompletedTask;
        }



        public Task<Block?> Raycast(int length = 100)
        {
            return Task.Run(() =>
            {

                var vc = this.Bot.BotEntity!.GetDirectionVector() / 10; // TODO: Vector kann block überspringen
                var lc = this.Bot.Player!.GetHeadPosition().Clone();

                for (var i = 0; i < 10 * length; i++)
                {
                    try
                    {
                        var b = this.World!.GetBlockAt(lc.Floored());
                        if (b.IsSolid())
                        {
                            var boundingBoxes = new List<AABB>();
                            foreach (var shape in b.GetBlockShape())
                            {
                                var bb = shape.ToBoundingBox();
                                bb.Offset(b.Position!.X, b.Position.Y, b.Position.Z);
                                if (bb.Contains(lc.X, lc.Y, lc.Z))
                                    return b;
                            }
                        }
                    } catch (ArgumentException)
                    {
                        return null;
                    }
                    lc.Add(vc);
                }
                return null;
            });
        }

        /// <summary>
        ///     Returns a Task that finishes once the Chunks in a square with length <paramref name="length" /> have been loaded
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public async Task WaitForChunksToLoad(int length = 5)
        {

            await this.Bot.WaitForBot();

            var playerChunk = this.World!.GetChunkCoordinates((int)this.Bot.BotEntity!.Position.X, (int)this.Bot.BotEntity.Position.Z);
            for (var x = playerChunk.X - length; x < playerChunk.X + length; x++)
            {
                for (var z = playerChunk.Z - length; z < playerChunk.Z + length; z++)
                {
                    while (this.World.GetChunkAt(new ChunkCoordinates(x, z)) == null) await Task.Delay(10);
                }
            }
        }

        /// <summary>
        ///     Tries to mine the block with the currently held item
        /// </summary>
        /// <param name="block"></param>
        /// <param name="face"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public Task<MineBlockStatus> MineBlock(Block block, BlockFace? face = null, CancellationToken? cancellation = null)
        {

            return Task.Run(async () =>
            {

                if (!block.Diggable) return MineBlockStatus.NotDiggable;
                if (!this.World!.IsBlockLoaded(block.Position!)) return MineBlockStatus.BlockNotLoaded;


                if (face == null)
                {
                    // TODO: Maybe rausfinden wie des geht   
                }

                if (cancellation?.IsCancellationRequested ?? false) return MineBlockStatus.Cancelled;

                var packet = new PacketBlockDig((int)DiggingStatus.StartedDigging, block.Position!.ToProtocolPosition(), (sbyte)(face ?? BlockFace.Top));

                await this.Bot.Client.SendPacket(packet);

                var time = block.CalculateBreakingTime(this.Bot.HeldItem, this.Bot.BotEntity!);

                var cancelToken = new CancellationTokenSource();

                cancellation?.Register(() => cancelToken.Cancel());

                var sendAgain = Task.Run(async () =>
                {
                    await Task.Delay(time, cancelToken.Token);
                    if (cancelToken.Token.IsCancellationRequested) return null;

                    packet.Status = (int)DiggingStatus.FinishedDigging;
                    await this.Bot.Client.SendPacket(packet);
                    return await this.Bot.WaitForPacket<PacketAcknowledgePlayerDigging>();
                });

                var ack = await this.Bot.WaitForPacket<PacketAcknowledgePlayerDigging>();
                if (cancellation?.IsCancellationRequested ?? false)
                {
                    cancelToken.Cancel();
                    await this.Bot.Client.SendPacket(new PacketBlockDig((int)DiggingStatus.CancelledDigging, block.Position!.ToProtocolPosition(), (sbyte)(face ?? BlockFace.Top)));
                    return MineBlockStatus.Cancelled;
                }

                if (ack.Status! != (int)DiggingStatus.StartedDigging)
                {
                    return MineBlockStatus.Failed;
                }

                var secondPacket = await sendAgain;
                if (secondPacket == null || !secondPacket.Successful)
                {

                    return MineBlockStatus.Failed;
                }

                return MineBlockStatus.Finished;
            });
        }
    }
}
