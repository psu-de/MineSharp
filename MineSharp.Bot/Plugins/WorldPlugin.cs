using MineSharp.Bot.Blocks;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Effects;
using MineSharp.Protocol.Packets.Clientbound.Play;
using MineSharp.Protocol.Packets.Serverbound.Play;
using MineSharp.World;
using MineSharp.World.Chunks;
using NLog;

namespace MineSharp.Bot.Plugins;

public class WorldPlugin : Plugin
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    
    public IWorld World { get; private set; }
    
    private PlayerPlugin? _playerPlugin;
    private WindowPlugin? _windowPlugin;

    public WorldPlugin(MinecraftBot bot) : base(bot)
    {
        this.World = WorldVersion.CreateWorld(this.Bot.Data);
        
        this.Bot.Client.On<ChunkDataAndUpdateLightPacket>(HandleChunkDataAndLightUpdatePacket);
        this.Bot.Client.On<UnloadChunkPacket>(this.HandleUnloadChunkPacket);
        this.Bot.Client.On<BlockUpdatePacket>(this.HandleBlockUpdatePacket);
        this.Bot.Client.On<MultiBlockUpdatePacket>(this.HandleMultiBlockUpdatePacket);
    }

    protected override Task Init()
    {
        this._playerPlugin = this.Bot.GetPlugin<PlayerPlugin>();
        this._windowPlugin = this.Bot.GetPlugin<WindowPlugin>();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Waits until all chunks in (radius * radius) are loaded around the bot.
    /// </summary>
    /// <param name="radius"></param>
    public async Task WaitForChunks(int radius = 5)
    {
        if (radius < 0)
            throw new ArgumentException($"{nameof(radius)} must be positive.");
        
        var player = this.Bot.GetPlugin<PlayerPlugin>();
        await player.WaitForInitialization();

        var chunkCoords = this.World!.ToChunkCoordinates(player.Entity!.Position);
        for (int x = chunkCoords.X - radius; x <= chunkCoords.X + radius; x++)
        {
            for (int z = chunkCoords.Z - radius; z <= chunkCoords.Z + radius; z++)
            {
                var coords = new ChunkCoordinates(x, z);
                while (!this.World.IsChunkLoaded(coords))
                    await Task.Delay(10);
            }
        }
    }

    public Task UpdateCommandBlock(Position location, string command, int mode, byte flags)
    {
        if (this._playerPlugin?.Self?.GameMode != GameMode.Creative)
        {
            throw new Exception("Player must be in creative mode.");
        }
        
        var packet = new UpdateCommandBlock(location, command, mode, flags);
        return this.Bot.Client.SendPacket(packet);
    }

    public async Task<MineBlockStatus> MineBlock(Block block, BlockFace? face = null, CancellationToken? cancellation = null)
    {
        if (this._playerPlugin?.Self == null)
            await this._playerPlugin?.WaitForInitialization()!;
        
        await this.WaitForChunks();
        
        if (!block.Info.Diggable)
            return MineBlockStatus.NotDiggable;

        if (6.0 < this._playerPlugin.Entity!.Position.DistanceTo((Vector3)block.Position))
            return MineBlockStatus.TooFar;
        
        if (!this.World!.IsBlockLoaded(block.Position))
            return MineBlockStatus.BlockNotLoaded;

        cancellation ??= CancellationToken.None;
        face ??= BlockFace.Top; // TODO: Figure out how to calculate BlockFace

        var time = this.CalculateBreakingTime(block);

        // first packet: Start digging
        var startPacket = new PlayerActionPacket( // TODO: PlayerActionPacket hardcoded values
            (int)DiggingStatus.StartedDigging,
            block.Position,
            face.Value,
            ++this.Bot.SequenceId); // Sequence Id is ignored when sending before 1.19

        var animationCts = new CancellationTokenSource();
        animationCts.CancelAfter(time);
        
        var diggingAnim = Task.Run(async () =>
        {
            while (true)
            {
                _ = this._playerPlugin.SwingArm();
                await Task.Delay(350, animationCts.Token);

                if (animationCts.IsCancellationRequested)
                    break;
            }
        }, animationCts.Token);
        
        try
        {
            await this.Bot.Client.SendPacket(startPacket, cancellation);

            // after waiting until the block has broken, another packet must be sent
            var sendAgain = Task.Run(async () =>
            {
                await Task.Delay(time);

                var finishPacket = new PlayerActionPacket(
                    (int)DiggingStatus.FinishedDigging,
                    block.Position,
                    face.Value,
                    ++this.Bot.SequenceId);

                var send = this.Bot.Client.SendPacket(finishPacket);
                var receive = this.Bot.Client.WaitForPacket<AcknowledgeBlockChangePacket>();

                await Task.WhenAll(send, receive);
                return receive.Result;
            }, cancellation.Value);

            var ack = await this.Bot.Client
                .WaitForPacket<AcknowledgeBlockChangePacket>()
                .WaitAsync(cancellation.Value);

            if (ack.Body is AcknowledgeBlockChangePacket.PacketBody_1_18 p118)
            {
                if ((DiggingStatus)p118.Status != DiggingStatus.StartedDigging)
                    return MineBlockStatus.Failed;
            } // TODO: MineBlock: What happens in 1.19?
                
            ack = await sendAgain;

            return ack.Body is AcknowledgeBlockChangePacket.PacketBody_1_18 { Successful: false } 
                ? MineBlockStatus.Failed 
                : MineBlockStatus.Finished;
        } catch (TaskCanceledException)
        {
            var cancelPacket = new PlayerActionPacket(
                (int)DiggingStatus.CancelledDigging,
                block.Position,
                face.Value,
                ++this.Bot.SequenceId);
            
            animationCts.Cancel();
            await this.Bot.Client.SendPacket(cancelPacket);

            return MineBlockStatus.Cancelled;
        }
    }

    public async Task PlaceBlock(Position position, PlayerHand hand = PlayerHand.MainHand, BlockFace face = BlockFace.Top)
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
            ++this.Bot.SequenceId);

        await Task.WhenAll(
            this.Bot.Client.SendPacket(packet),
            this._playerPlugin!.SwingArm());
    }
    
    private int CalculateBreakingTime(Block block)
    {
        if (this._playerPlugin?.Self?.GameMode == GameMode.Creative)
            return 0;
        
        var heldItem = this._windowPlugin?.HeldItem;
        float toolMultiplier = 1;

        if (heldItem != null)
        {
            toolMultiplier = block.Info.Materials
                .Select(x => this.Bot.Data.Materials.GetToolMultiplier(x, heldItem.Info.Type))
                .Max();
        }
            
        float efficiencyLevel = 0; // TODO: Efficiency level
        float hasteLevel = this._playerPlugin?.Entity?.GetEffectLevel(EffectType.Haste) ?? 0;
        float miningFatigueLevel = this._playerPlugin?.Entity?.GetEffectLevel(EffectType.MiningFatigue) ?? 0;

        toolMultiplier /= MathF.Pow(1.3f, efficiencyLevel);
        toolMultiplier /= MathF.Pow(1.2f, hasteLevel);
        toolMultiplier *= MathF.Pow(0.3f, miningFatigueLevel);

        var damage = toolMultiplier / block.Info.Hardness;

        var canHarvest = block.CanBeHarvestedBy(heldItem?.Info.Type);
        damage /= canHarvest ? 30.0f : 100.0f;

        if (damage > 1) return 0;

        var ticks = MathF.Ceiling(1 / damage);
        return (int)(ticks / 20 * 1000);
    }

    private Task HandleChunkDataAndLightUpdatePacket(ChunkDataAndUpdateLightPacket packet)
    {
        if (!this.IsEnabled)
            return Task.CompletedTask;
        
        var coords = new ChunkCoordinates(packet.X, packet.Z);
        var chunk = WorldVersion.CreateChunk(this.Bot.Data, coords, packet.BlockEntities);
        chunk.LoadData(packet.ChunkData);
        
        this.World!.LoadChunk(chunk);
        
        return Task.CompletedTask;
    }

    private Task HandleUnloadChunkPacket(UnloadChunkPacket packet)
    {        
        if (!this.IsEnabled)
            return Task.CompletedTask;
        
        var coords = new ChunkCoordinates(packet.X, packet.Z);
        
        this.World!.UnloadChunk(coords);
        return Task.CompletedTask;
    }

    private Task HandleBlockUpdatePacket(BlockUpdatePacket packet)
    {
        if (!this.IsEnabled)
            return Task.CompletedTask;
        
        var blockInfo = this.Bot.Data.Blocks.GetByState(packet.StateId);
        var block = new Block(blockInfo, packet.StateId, packet.Location);

        this.World!.SetBlock(block);
        return Task.CompletedTask;
    }

    private Task HandleMultiBlockUpdatePacket(MultiBlockUpdatePacket packet)
    {
        if (!this.IsEnabled)
            return Task.CompletedTask;
        
        var sectionX = packet.ChunkSection >> 42;
        var sectionY = packet.ChunkSection << 44 >> 44;
        var sectionZ = packet.ChunkSection << 22 >> 42;
        
        if (sectionX > Math.Pow(2, 21)) 
            sectionX -= (int)Math.Pow(2, 22);
        
        if (sectionY > Math.Pow(2, 19)) 
            sectionY -= (int)Math.Pow(2, 20);
        
        if (sectionZ > Math.Pow(2, 21)) 
            sectionZ -= (int)Math.Pow(2, 22);

        var coords = new ChunkCoordinates((int)sectionX, (int)sectionZ);
        var chunk = this.World!.GetChunkAt(coords);

        foreach (var l in packet.Blocks)
        {
            var blockZ = (int)(l >> 4 & 0x0F);
            var blockX = (int)(l >> 8 & 0x0F);
            var blockY = (int)(l & 0x0F) + (int)sectionY * IChunk.SIZE;
            var stateId = (int)(l >> 12);

            chunk.SetBlockAt(stateId, new Position(blockX, blockY, blockZ));
        }

        return Task.CompletedTask;
    }
}
