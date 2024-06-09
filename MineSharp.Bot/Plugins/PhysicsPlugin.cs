using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Entities;
using MineSharp.Core.Geometry;
using MineSharp.Physics;
using MineSharp.Physics.Input;
using MineSharp.Protocol.Packets.Serverbound.Play;
using MineSharp.World.Iterators;
using NLog;

namespace MineSharp.Bot.Plugins;

/// <summary>
/// Physics plugin simulates the player entity in the world.
/// It also allows for walking, jumping and crouching.
/// </summary>
public class PhysicsPlugin : Plugin
{
    private const           float   ROTATION_SMOOTHNESS = 0.2f;
    private const           double  POSITION_THRESHOLD  = 0.01d;
    
    private static readonly ILogger Logger              = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Fired when the Bot moves
    /// </summary>
    public event Events.BotEvent? BotMoved;

    /// <summary>
    ///     Fires just before executing a physics tick
    /// </summary>
    public event Events.BotEvent? PhysicsTick;

    /// <summary>
    /// The input controls used to control movement.
    /// </summary>
    public readonly InputControls InputControls;

    /// <summary>
    /// The Physics engine for this plugin
    /// </summary>
    public PlayerPhysics? Engine { get; private set; }

    private PlayerState   lastPlayerState;
    private PlayerPlugin? playerPlugin;
    private WorldPlugin?  worldPlugin;

    private MinecraftPlayer? Self;

    private LerpRotation? lerpRotation;

    private uint tickCounter = 0;

    /// <summary>
    /// Create a new PhysicsPlugin instance
    /// </summary>
    /// <param name="bot"></param>
    public PhysicsPlugin(MineSharpBot bot) : base(bot)
    {
        this.lastPlayerState = new PlayerState(0, 0, 0, 0, 0, false);
        this.InputControls   = new InputControls();
    }

    /// <inheritdoc />
    protected override async Task Init()
    {
        this.playerPlugin = this.Bot.GetPlugin<PlayerPlugin>();
        this.worldPlugin  = this.Bot.GetPlugin<WorldPlugin>();

        await this.playerPlugin.WaitForInitialization();

        this.Self = this.playerPlugin.Self;
        await this.UpdateServerPos();

        this.Engine                    =  new PlayerPhysics(this.Bot.Data, this.Self!, this.worldPlugin.World, this.InputControls);
        this.Engine.OnCrouchingChanged += OnSneakingChanged;
        this.Engine.OnSprintingChanged += OnSprintingChanged;
    }

    /// <summary>
    /// Wait for <paramref name="count"/> physics ticks
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public async Task WaitForTick(int count = 1)
    {
        var before   = this.tickCounter;
        var expected = before + count;

        while (this.tickCounter < expected)
        {
            await Task.Delay(1);
        }
    }

    /// <summary>
    /// Wait until the entity hit the ground
    /// </summary>
    public async Task WaitForOnGround()
    {
        while (!this.playerPlugin?.Entity!.IsOnGround ?? false)
        {
            await this.WaitForTick();
        }
    }

    /// <summary>
    /// Forces the bots rotation to the given yaw and pitch (in degrees)
    /// </summary>
    /// <param name="yaw"></param>
    /// <param name="pitch"></param>
    public void ForceSetRotation(float yaw, float pitch)
    {
        this.Self!.Entity!.Yaw   = yaw;
        this.Self!.Entity!.Pitch = pitch;
    }

    /// <summary>
    /// Forces the bot to look at given position
    /// </summary>
    /// <param name="position"></param>
    public void ForceLookAt(Vector3 position)
    {
        (var yaw, var pitch) = this.CalculateRotation(position);
        this.ForceSetRotation(yaw, pitch);
    }

    /// <summary>
    /// Forces the bot to look at the center of the given block
    /// </summary>
    /// <param name="block"></param>
    public void ForceLookAt(Block block)
        => this.ForceLookAt(new Vector3(0.5, 0.5, 0.5).Plus(block.Position));

    /// <summary>
    /// Slowly look at the given yaw and pitch.
    /// Use a higher smoothness value for faster rotation,
    /// lower value for slower rotation.
    /// </summary>
    /// <param name="yaw"></param>
    /// <param name="pitch"></param>
    /// <param name="smoothness"></param>
    public async Task Look(float yaw, float pitch, float smoothness = ROTATION_SMOOTHNESS)
    {
        var dYaw   = yaw   - this.Self!.Entity!.Yaw;
        var dPitch = pitch - this.Self!.Entity!.Pitch;

        if (Math.Abs(dYaw) < 0.1 && Math.Abs(dPitch) < 0.1)
            return;

        this.lerpRotation?.Cancel();
        this.lerpRotation = new LerpRotation(this.playerPlugin!.Self!, yaw, pitch, smoothness);

        await this.lerpRotation.GetTask();

        this.lerpRotation = null;
    }

    /// <summary>
    /// Slowly look at the given position.
    /// Use a higher smoothness value for faster rotation,
    /// a lower value for slower rotation.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="smoothness"></param>
    /// <returns></returns>
    public Task LookAt(Vector3 position, float smoothness = ROTATION_SMOOTHNESS)
    {
        var rotation = this.CalculateRotation(position);
        return this.Look(rotation.Yaw, rotation.Pitch);
    }

    /// <summary>
    /// Slowly look at the center of the given block.
    /// Use a higher smoothness value for faster rotation,
    /// a lower value for slower rotation.
    /// </summary>
    /// <param name="block"></param>
    /// <param name="smoothness"></param>
    /// <returns></returns>
    public Task LookAt(Block block, float smoothness = ROTATION_SMOOTHNESS)
        => this.LookAt(new Vector3(0.5, 0.5, 0.5).Plus(block.Position), smoothness);

    /// <summary>
    /// Casts a ray from the players eyes, and returns the first block that is hit.
    /// </summary>
    /// <returns></returns>
    public (Block Block, BlockFace Face)? Raycast(double distance = 64)
    {
        if (distance < 0)
        {
            return null;
        }
        
        var position   = this.playerPlugin!.Self!.GetHeadPosition();
        var lookVector = this.playerPlugin!.Self!.Entity!.GetLookVector();
        var iterator = new RaycastIterator(
            position,
            lookVector,
            length: distance);

        foreach (var pos in iterator.Iterate())
        {
            var block = this.worldPlugin!.World.GetBlockAt(pos);
            if (!block.IsSolid())
            {
                continue;
            }

            var bbs = this.Bot.Data.BlockCollisionShapes.GetForBlock(block);

            foreach (var bb in bbs)
            {
                var blockBb = bb.Clone().Offset(block.Position.X, block.Position.Y, block.Position.Z);

                if (blockBb.IntersectsLine(position, lookVector))
                {
                    return (block, iterator.CurrentFace);
                }
            }
        }

        return null;
    }

    /// <inheritdoc />
    public override Task OnTick()
    {
        if (!this.IsLoaded)
            return Task.CompletedTask;

        if (!this.playerPlugin!.IsAlive!.Value)
            return Task.CompletedTask;

        _ = Task.Run(async () =>
        {
            try
            {
                this.lerpRotation?.Tick();
                this.Engine!.Tick();
                await this.UpdateServerPositionIfNeeded();

                this.PhysicsTick?.Invoke(this.Bot);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
            }

            this.tickCounter++;
        });

        return Task.CompletedTask;
    }

    private (float Yaw, float Pitch) CalculateRotation(Vector3 position)
    {
        const double deg2rad = 180 / Math.PI;
        var          delta   = position.Minus(this.Self!.GetHeadPosition());
        delta.Normalize();

        var yaw   = deg2rad * Math.Atan2(-delta.X, delta.Z);
        var pitch = deg2rad * -Math.Asin(delta.Y / delta.Length());

        return ((float)yaw, (float)pitch);
    }

    private async Task UpdateServerPositionIfNeeded()
    {
        if (Math.Abs(this.lastPlayerState.X     - this.Self!.Entity!.Position.X) > POSITION_THRESHOLD
         || Math.Abs(this.lastPlayerState.Y     - this.Self!.Entity!.Position.Y) > POSITION_THRESHOLD
         || Math.Abs(this.lastPlayerState.Z     - this.Self!.Entity!.Position.Z) > POSITION_THRESHOLD
         || Math.Abs(this.lastPlayerState.Yaw   - this.Self!.Entity!.Yaw)        > POSITION_THRESHOLD
         || Math.Abs(this.lastPlayerState.Pitch - this.Self!.Entity!.Pitch)      > POSITION_THRESHOLD
         || this.lastPlayerState.OnGround                                        != this.Self!.Entity!.IsOnGround)
        {
            await this.UpdateServerPos();
        }
    }

    private async Task UpdateServerPos()
    {
        var packet = new SetPlayerPositionAndRotationPacket(
            this.Self!.Entity!.Position.X,
            this.Self!.Entity!.Position.Y,
            this.Self!.Entity!.Position.Z,
            this.Self!.Entity!.Yaw,
            this.Self!.Entity!.Pitch,
            this.Self!.Entity!.IsOnGround);

        this.lastPlayerState.X        = this.Self!.Entity!.Position.X;
        this.lastPlayerState.Y        = this.Self!.Entity!.Position.Y;
        this.lastPlayerState.Z        = this.Self!.Entity!.Position.Z;
        this.lastPlayerState.Yaw      = this.Self!.Entity!.Yaw;
        this.lastPlayerState.Pitch    = this.Self!.Entity!.Pitch;
        this.lastPlayerState.OnGround = this.Self!.Entity!.IsOnGround;

        await this.Bot.Client.SendPacket(packet);

        this.BotMoved?.Invoke(this.Bot);
    }

    private void OnSneakingChanged(PlayerPhysics sender, bool isSneaking)
    {
        var packet = new EntityActionPacket(
            this.playerPlugin!.Self!.Entity!.ServerId,
            isSneaking
                ? EntityActionPacket.EntityAction.StartSneaking
                : EntityActionPacket.EntityAction.StopSneaking,
            0);

        this.Bot.Client.SendPacket(packet);
    }

    private void OnSprintingChanged(PlayerPhysics sender, bool isSprinting)
    {
        var packet = new EntityActionPacket(
            this.playerPlugin!.Self!.Entity!.ServerId,
            isSprinting
                ? EntityActionPacket.EntityAction.StartSprinting
                : EntityActionPacket.EntityAction.StopSprinting,
            0);

        this.Bot.Client.SendPacket(packet);
    }

    private record PlayerState
    {
        public double X        { get; set; }
        public double Y        { get; set; }
        public double Z        { get; set; }
        public float  Yaw      { get; set; }
        public float  Pitch    { get; set; }
        public bool   OnGround { get; set; }

        public PlayerState(double x, double y, double z, float yaw, float pitch, bool ground)
        {
            this.X        = x;
            this.Y        = y;
            this.Z        = z;
            this.Yaw      = yaw;
            this.Pitch    = pitch;
            this.OnGround = ground;
        }

        public override string ToString() =>
            $"X={this.X} Y={this.Y} Z={this.Z} Yaw={this.Yaw} Pitch={this.Pitch} IsOnGround={this.OnGround}";
    }

    private class LerpRotation
    {
        private MinecraftPlayer      player;
        private float                yawPerTick;
        private float                pitchPerTick;
        private int                  remainingYawTicks;
        private int                  remainingPitchTicks;
        private bool                 completed;
        private TaskCompletionSource task;

        public LerpRotation(MinecraftPlayer player, float toYaw, float toPitch, float smoothness = ROTATION_SMOOTHNESS)
        {
            this.player = player;

            var deltaYaw   = toYaw   - player.Entity!.Yaw;
            var deltaPitch = toPitch - player.Entity!.Pitch;

            this.yawPerTick   = deltaYaw   * smoothness;
            this.pitchPerTick = deltaPitch * smoothness;

            if (deltaYaw == 0)
                yawPerTick = 1;
            if (deltaPitch == 0)
                pitchPerTick = 1;

            var yawTicks   = Math.Abs((int) (deltaYaw   / yawPerTick));
            var pitchTicks = Math.Abs((int) (deltaPitch / pitchPerTick));

            this.task                = new TaskCompletionSource();
            this.remainingYawTicks   = yawTicks;
            this.remainingPitchTicks = pitchTicks;
        }

        public void Tick()
        {
            if (this.completed)
                return;

            if (this.remainingPitchTicks > 0)
            {
                this.remainingPitchTicks--;
                this.player.Entity!.Pitch += this.pitchPerTick;
            }

            if (this.remainingYawTicks > 0)
            {
                this.remainingYawTicks--;
                this.player.Entity!.Yaw += this.yawPerTick;
            }

            if (this.remainingPitchTicks != 0 || this.remainingYawTicks != 0)
                return;

            this.completed = true;
            this.task.TrySetResult();
        }

        public void Cancel()
        {
            this.completed = true;
            this.task.TrySetCanceled();
        }

        public Task GetTask()
            => this.task.Task;
    }
}
