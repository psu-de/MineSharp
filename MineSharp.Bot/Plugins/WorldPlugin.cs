using MineSharp.Bot.Blocks;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Effects;
using MineSharp.Core.Geometry;
using MineSharp.Protocol.Packets.Clientbound.Play;
using MineSharp.Protocol.Packets.Serverbound.Play;
using MineSharp.World;
using MineSharp.World.Chunks;
using NLog;

namespace MineSharp.Bot.Plugins;

/// <summary>
///     World plugin handles all kind of packets regarding the Minecraft world,
///     and provides methods to interact with it, like mining and digging.
/// </summary>
public class WorldPlugin : Plugin
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private DateTime? chunkBatchStart;

    private PlayerPlugin? playerPlugin;
    private WindowPlugin? windowPlugin;

    /// <summary>
    ///     Create a new WorldPlugin instance
    /// </summary>
    /// <param name="bot"></param>
    public WorldPlugin(MineSharpBot bot) : base(bot)
    {
        // we want all the packets. Even those that are sent before the plugin is initialized.
        OnPacketAfterInitialization<ChunkDataAndUpdateLightPacket>(HandleChunkDataAndLightUpdatePacket, true);
        OnPacketAfterInitialization<UnloadChunkPacket>(HandleUnloadChunkPacket, true);
        OnPacketAfterInitialization<BlockUpdatePacket>(HandleBlockUpdatePacket, true);
        OnPacketAfterInitialization<MultiBlockUpdatePacket>(HandleMultiBlockUpdatePacket, true);
        OnPacketAfterInitialization<ChunkBatchStartPacket>(HandleChunkBatchStartPacket, true);
        OnPacketAfterInitialization<ChunkBatchFinishedPacket>(HandleChunkBatchFinishedPacket, true);
    }

    /// <summary>
    ///     The world of the Minecraft server
    /// </summary>
    public IAsyncWorld? World { get; private set; }

    /// <inheritdoc />
    protected override async Task Init()
    {
        playerPlugin = Bot.GetPlugin<PlayerPlugin>();
        windowPlugin = Bot.GetPlugin<WindowPlugin>();

        await playerPlugin.WaitForInitialization().WaitAsync(Bot.CancellationToken);
        var dimension = playerPlugin.Self!.Dimension;
        World = WorldVersion.CreateWorld(Bot.Data, dimension);
    }

    /// <summary>
    ///     Waits until all chunks in (radius * radius) are loaded around the bot.
    /// </summary>
    /// <param name="radius"></param>
    public async Task WaitForChunks(int radius = 5)
    {
        if (radius < 0)
        {
            throw new ArgumentException($"{nameof(radius)} must be positive.");
        }

        var player = Bot.GetPlugin<PlayerPlugin>();
        await player.WaitForInitialization();

        var chunkCoords = World!.ToChunkCoordinates((Position)player.Entity!.Position);
        for (var x = chunkCoords.X - radius; x <= chunkCoords.X + radius; x++)
        {
            for (var z = chunkCoords.Z - radius; z <= chunkCoords.Z + radius; z++)
            {
                var coords = new ChunkCoordinates(x, z);
                while (!World!.IsChunkLoaded(coords))
                {
                    await Task.Delay(10);
                }
            }
        }
    }

    /// <summary>
    ///     Update the Command block at the given position
    /// </summary>
    /// <param name="location"></param>
    /// <param name="command"></param>
    /// <param name="mode"></param>
    /// <param name="flags"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public Task UpdateCommandBlock(Position location, string command, int mode, byte flags)
    {
        if (playerPlugin?.Self?.GameMode != GameMode.Creative)
        {
            throw new("Player must be in creative mode.");
        }

        var packet = new UpdateCommandBlock(location, command, mode, flags);
        return Bot.Client.SendPacket(packet);
    }

    /// <summary>
    ///     Mine the given block
    /// </summary>
    /// <param name="block">The block to mine</param>
    /// <param name="face">The block face the bot is facing</param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    public async Task<MineBlockStatus> MineBlock(Block block, BlockFace? face = null,
                                                 CancellationToken cancellation = default)
    {
        if (playerPlugin?.Self == null)
        {
            await playerPlugin?.WaitForInitialization()!;
        }

        await WaitForChunks();

        if (block.Info.Unbreakable)
        {
            return MineBlockStatus.NotDiggable;
        }

        if (6.0 < playerPlugin.Entity!.Position.DistanceTo(block.Position))
        {
            return MineBlockStatus.TooFar;
        }

        if (!World!.IsBlockLoaded(block.Position))
        {
            return MineBlockStatus.BlockNotLoaded;
        }

        face ??= BlockFace.Top; // TODO: Figure out how to calculate BlockFace

        var time = CalculateBreakingTime(block);

        // first packet: Start digging
        var startPacket = new PlayerActionPacket( // TODO: PlayerActionPacket hardcoded values
            (int)PlayerActionStatus.StartedDigging,
            block.Position,
            face.Value,
            ++Bot.SequenceId); // Sequence Id is ignored when sending before 1.19

        var animationCts = new CancellationTokenSource();
        animationCts.CancelAfter(time);

        var diggingAnim = Task.Run(async () =>
        {
            while (true)
            {
                _ = playerPlugin.SwingArm();
                await Task.Delay(350, animationCts.Token);

                if (animationCts.IsCancellationRequested)
                {
                    break;
                }
            }
        }, animationCts.Token);

        try
        {
            await Bot.Client.SendPacket(startPacket, cancellation);

            // after waiting until the block has broken, another packet must be sent
            var sendAgain = Task.Run(async () =>
            {
                await Task.Delay(time);

                var finishPacket = new PlayerActionPacket(
                    (int)PlayerActionStatus.FinishedDigging,
                    block.Position,
                    face.Value,
                    ++Bot.SequenceId);

                var send = Bot.Client.SendPacket(finishPacket, cancellation);
                var receive = Bot.Client.WaitForPacket<AcknowledgeBlockChangePacket>();

                await Task.WhenAll(send, receive);
                return receive.Result;
            }, cancellation);

            var ack = await Bot.Client
                               .WaitForPacket<AcknowledgeBlockChangePacket>()
                               .WaitAsync(cancellation);

            if (ack.Body is AcknowledgeBlockChangePacket.PacketBody118 p118)
            {
                if ((PlayerActionStatus)p118.Status != PlayerActionStatus.StartedDigging)
                {
                    return MineBlockStatus.Failed;
                }
            } // TODO: MineBlock: What happens in 1.19?

            ack = await sendAgain;

            return ack.Body is AcknowledgeBlockChangePacket.PacketBody118 { Successful: false }
                ? MineBlockStatus.Failed
                : MineBlockStatus.Finished;
        }
        catch (TaskCanceledException)
        {
            var cancelPacket = new PlayerActionPacket(
                (int)PlayerActionStatus.CancelledDigging,
                block.Position,
                face.Value,
                ++Bot.SequenceId);

            animationCts.Cancel();
            await Bot.Client.SendPacket(cancelPacket);

            return MineBlockStatus.Cancelled;
        }
    }

    /// <summary>
    ///     Place the block the bot is currently holding at the given position
    /// </summary>
    /// <param name="position"></param>
    /// <param name="hand"></param>
    /// <param name="face"></param>
    public async Task PlaceBlock(Position position, PlayerHand hand = PlayerHand.MainHand,
                                 BlockFace face = BlockFace.Top)
    {
        // TODO: PlaceBlock: Hardcoded values
        var packet = new PlaceBlockPacket(
            (int)hand,
            position,
            face,
            0.5f,
            0.5f,
            0.5f,
            false,
            ++Bot.SequenceId);

        await Task.WhenAll(
            Bot.Client.SendPacket(packet),
            playerPlugin!.SwingArm());
    }

    private int CalculateBreakingTime(Block block)
    {
        if (playerPlugin?.Self?.GameMode == GameMode.Creative)
        {
            return 0;
        }

        var heldItem = windowPlugin?.HeldItem;
        float toolMultiplier = 1;

        if (heldItem != null)
        {
            toolMultiplier = block.Info.Materials
                                  .Select(x => Bot.Data.Materials.GetMultiplier(x, heldItem.Info.Type))
                                  .Max();
        }

        float efficiencyLevel = 0; // TODO: Efficiency level
        float hasteLevel = playerPlugin?.Entity?.GetEffectLevel(EffectType.Haste) ?? 0;
        float miningFatigueLevel = playerPlugin?.Entity?.GetEffectLevel(EffectType.MiningFatigue) ?? 0;

        toolMultiplier /= MathF.Pow(1.3f, efficiencyLevel);
        toolMultiplier /= MathF.Pow(1.2f, hasteLevel);
        toolMultiplier *= MathF.Pow(0.3f, miningFatigueLevel);

        var damage = toolMultiplier / block.Info.Hardness;

        var canHarvest = block.CanBeHarvestedBy(heldItem?.Info.Type);
        damage /= canHarvest ? 30.0f : 100.0f;

        if (damage > 1)
        {
            return 0;
        }

        var ticks = MathF.Ceiling(1 / damage);
        return (int)(ticks / 20 * 1000);
    }

    private Task HandleChunkDataAndLightUpdatePacket(ChunkDataAndUpdateLightPacket packet)
    {
        if (!IsEnabled)
        {
            return Task.CompletedTask;
        }

        var coords = new ChunkCoordinates(packet.X, packet.Z);
        var chunk = World!.CreateChunk(coords, packet.BlockEntities);
        chunk.LoadData(packet.ChunkData);

        World!.LoadChunk(chunk);

        return Task.CompletedTask;
    }

    private Task HandleUnloadChunkPacket(UnloadChunkPacket packet)
    {
        if (!IsEnabled)
        {
            return Task.CompletedTask;
        }

        var coords = new ChunkCoordinates(packet.X, packet.Z);

        World!.UnloadChunk(coords);
        return Task.CompletedTask;
    }

    private Task HandleBlockUpdatePacket(BlockUpdatePacket packet)
    {
        if (!IsEnabled)
        {
            return Task.CompletedTask;
        }

        var blockInfo = Bot.Data.Blocks.ByState(packet.StateId)!;
        var block = new Block(blockInfo, packet.StateId, packet.Location);

        return World!.SetBlockAsync(block);
    }

    private async Task HandleMultiBlockUpdatePacket(MultiBlockUpdatePacket packet)
    {
        if (!IsEnabled)
        {
            return;
        }

        var sectionX = packet.ChunkSection >> (64 - 22); // first 22 bits
        var sectionZ = (packet.ChunkSection << 22) >> (64 - 22); // next 22 bits
        var sectionY = (packet.ChunkSection << (22 + 22)) >> (64 - 20); // last 20 bits

        var coords = new ChunkCoordinates((int)sectionX, (int)sectionZ);
        var chunk = await World!.GetChunkAtAsync(coords);

        foreach (var l in packet.Blocks)
        {
            var blockZ = (int)((l >> 4) & 0x0F);
            var blockX = (int)((l >> 8) & 0x0F);
            var blockY = (int)(l & 0x0F) + ((int)sectionY * IChunk.Size);
            var stateId = (int)(l >> 12);

            chunk.SetBlockAt(stateId, new(blockX, blockY, blockZ));
        }
    }

    private Task HandleChunkBatchStartPacket(ChunkBatchStartPacket packet)
    {
        if (chunkBatchStart != null)
        {
            Logger.Warn("Received ChunkBatchStartPacket, but last chunk batch was not cleared yet.");
        }

        chunkBatchStart = DateTime.Now;
        return Task.CompletedTask;
    }

    private async Task HandleChunkBatchFinishedPacket(ChunkBatchFinishedPacket packet)
    {
        if (!chunkBatchStart.HasValue)
        {
            Logger.Warn("Received ChunkBatchFinishedPacket, but no chunk batch was started.");
            return;
        }

        var timeSpan = DateTime.Now - chunkBatchStart!.Value;
        chunkBatchStart = null;

        var cps = 25_000.0f / timeSpan.Microseconds;

        if (float.IsNaN(cps) || float.IsInfinity(cps))
        {
            cps = 5;
        }

        await Bot.Client.SendPacket(new ChunkBatchReceivedPacket(cps));
    }
}
