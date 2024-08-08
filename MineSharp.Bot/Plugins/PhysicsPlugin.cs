using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Entities;
using MineSharp.Core.Events;
using MineSharp.Core.Geometry;
using MineSharp.Physics;
using MineSharp.Physics.Input;
using MineSharp.Protocol.Packets.Serverbound.Play;
using MineSharp.World.Iterators;
using NLog;

namespace MineSharp.Bot.Plugins;

/// <summary>
///     Physics plugin simulates the player entity in the world.
///     It also allows for walking, jumping and crouching.
/// </summary>
public class PhysicsPlugin : Plugin
{
    private const float RotationSmoothness = 0.2f;
    private const double PositionThreshold = 0.01d;

    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    ///     The input controls used to control movement.
    /// </summary>
    public readonly InputControls InputControls;

    private readonly PlayerState lastPlayerState;

    private LerpRotation? lerpRotation;
    private PlayerPlugin? playerPlugin;

    private MinecraftPlayer? self;

    private uint tickCounter;
    private WorldPlugin? worldPlugin;

    /// <summary>
    ///     Create a new PhysicsPlugin instance
    /// </summary>
    /// <param name="bot"></param>
    public PhysicsPlugin(MineSharpBot bot) : base(bot)
    {
        lastPlayerState = new(0, 0, 0, 0, 0, false);
        InputControls = new();
    }

    /// <summary>
    ///     The Physics engine for this plugin
    /// </summary>
    public PlayerPhysics? Engine { get; private set; }

    /// <summary>
    ///     Fired when the Bot moves
    /// </summary>
    public AsyncEvent<MineSharpBot> BotMoved = new();

    /// <summary>
    ///     Fires just before executing a physics tick
    /// </summary>
    public AsyncEvent<MineSharpBot> PhysicsTick = new();

    /// <inheritdoc />
    protected override async Task Init()
    {
        playerPlugin = Bot.GetPlugin<PlayerPlugin>();
        worldPlugin = Bot.GetPlugin<WorldPlugin>();

        await playerPlugin.WaitForInitialization().WaitAsync(Bot.CancellationToken);
        await worldPlugin.WaitForInitialization().WaitAsync(Bot.CancellationToken);

        self = playerPlugin.Self;
        await UpdateServerPos().WaitAsync(Bot.CancellationToken);

        Engine = new(Bot.Data, self!, worldPlugin.World!, InputControls);
        Engine.OnCrouchingChanged += OnSneakingChanged;
        Engine.OnSprintingChanged += OnSprintingChanged;
    }

    /// <summary>
    ///     Wait for <paramref name="count" /> physics ticks
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public async Task WaitForTick(int count = 1)
    {
        var before = tickCounter;
        var expected = before + count;

        while (tickCounter < expected)
        {
            await Task.Delay(1);
        }
    }

    /// <summary>
    ///     Wait until the entity hit the ground
    /// </summary>
    public async Task WaitForOnGround()
    {
        while (!playerPlugin?.Entity!.IsOnGround ?? false)
        {
            await WaitForTick();
        }
    }

    /// <summary>
    ///     Forces the bots rotation to the given yaw and pitch (in degrees)
    /// </summary>
    /// <param name="yaw"></param>
    /// <param name="pitch"></param>
    public void ForceSetRotation(float yaw, float pitch)
    {
        self!.Entity!.Yaw = yaw;
        self!.Entity!.Pitch = pitch;
    }

    /// <summary>
    ///     Forces the bot to look at given position
    /// </summary>
    /// <param name="position"></param>
    public void ForceLookAt(Vector3 position)
    {
        (var yaw, var pitch) = CalculateRotation(position);
        ForceSetRotation(yaw, pitch);
    }

    /// <summary>
    ///     Forces the bot to look at the center of the given block
    /// </summary>
    /// <param name="block"></param>
    public void ForceLookAt(Block block)
    {
        ForceLookAt(new Vector3(0.5, 0.5, 0.5).Plus(block.Position));
    }

    /// <summary>
    ///     Slowly look at the given yaw and pitch.
    ///     Use a higher smoothness value for faster rotation,
    ///     lower value for slower rotation.
    /// </summary>
    /// <param name="yaw"></param>
    /// <param name="pitch"></param>
    /// <param name="smoothness"></param>
    public async Task Look(float yaw, float pitch, float smoothness = RotationSmoothness)
    {
        var dYaw = yaw - self!.Entity!.Yaw;
        var dPitch = pitch - self!.Entity!.Pitch;

        if (Math.Abs(dYaw) < 0.1 && Math.Abs(dPitch) < 0.1)
        {
            return;
        }

        lerpRotation?.Cancel();
        lerpRotation = new(playerPlugin!.Self!, yaw, pitch, smoothness);

        await lerpRotation.GetTask();

        lerpRotation = null;
    }

    /// <summary>
    ///     Slowly look at the given position.
    ///     Use a higher smoothness value for faster rotation,
    ///     a lower value for slower rotation.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="smoothness"></param>
    /// <returns></returns>
    public Task LookAt(Vector3 position, float smoothness = RotationSmoothness)
    {
        var rotation = CalculateRotation(position);
        return Look(rotation.Yaw, rotation.Pitch);
    }

    /// <summary>
    ///     Slowly look at the center of the given block.
    ///     Use a higher smoothness value for faster rotation,
    ///     a lower value for slower rotation.
    /// </summary>
    /// <param name="block"></param>
    /// <param name="smoothness"></param>
    /// <returns></returns>
    public Task LookAt(Block block, float smoothness = RotationSmoothness)
    {
        return LookAt(new Vector3(0.5, 0.5, 0.5).Plus(block.Position), smoothness);
    }

    /// <summary>
    ///     Represents the result of a ray casting operation.
    /// </summary>
    /// <param name="Block">The block that was hit by the ray.</param>
    /// <param name="Face">The face of the block that was hit.</param>
    /// <param name="BlockCollisionShapeIndex">The index for the collision shape of the block that was hit. You can get the <see cref="Aabb"/> with: <c>MineSharp.Bot.Data.BlockCollisionShapes.GetForBlock(block)</c>.</param>
    /// <param name="Distance">The distance from the ray origin to the hit point.</param>
    public record RaycastBlockResult(Block Block, BlockFace Face, int BlockCollisionShapeIndex, double Distance);

    /// <summary>
    ///     Casts a ray from the players eyes, and returns the first block that is hit.
    /// </summary>
    /// <returns></returns>
    public RaycastBlockResult? Raycast(double distance = 64)
    {
        if (distance < 0)
        {
            return null;
        }

        var position = playerPlugin!.Self!.GetHeadPosition();
        var lookVector = playerPlugin!.Self!.Entity!.GetLookVector();
        var iterator = new RaycastIterator(
            position,
            lookVector,
            distance);

        foreach (var pos in iterator.Iterate())
        {
            var block = worldPlugin!.World!.GetBlockAt(pos);
            if (!block.IsSolid())
            {
                continue;
            }

            var bbs = Bot.Data.BlockCollisionShapes.GetForBlock(block);

            for (int bbIndex = 0; bbIndex < bbs.Length; bbIndex++)
            {
                var bb = bbs[bbIndex];
                var blockBb = bb.Clone().Offset(block.Position.X, block.Position.Y, block.Position.Z);

                var intersectionDistance = blockBb.IntersectsLine(position, lookVector);
                if (intersectionDistance is not null)
                {
                    return new(block, iterator.CurrentFace, bbIndex, intersectionDistance.Value);
                }
            }
        }

        return null;
    }

    /// <inheritdoc />
    protected internal override Task OnTick()
    {
        if (!IsLoaded)
        {
            return Task.CompletedTask;
        }

        if (!playerPlugin!.IsAlive!.Value)
        {
            return Task.CompletedTask;
        }

        _ = Task.Run(async () =>
        {
            try
            {
                lerpRotation?.Tick();
                Engine!.Tick();
                await UpdateServerPositionIfNeeded();

                await PhysicsTick.Dispatch(Bot);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
            }

            tickCounter++;
        });

        return Task.CompletedTask;
    }

    private (float Yaw, float Pitch) CalculateRotation(Vector3 position)
    {
        const double deg2_rad = 180 / Math.PI;
        var delta = position.Minus(self!.GetHeadPosition());
        delta.Normalize();

        var yaw = deg2_rad * Math.Atan2(-delta.X, delta.Z);
        var pitch = deg2_rad * -Math.Asin(delta.Y / delta.Length());

        return ((float)yaw, (float)pitch);
    }

    private async Task UpdateServerPositionIfNeeded()
    {
        if (Math.Abs(lastPlayerState.X - self!.Entity!.Position.X) > PositionThreshold
            || Math.Abs(lastPlayerState.Y - self!.Entity!.Position.Y) > PositionThreshold
            || Math.Abs(lastPlayerState.Z - self!.Entity!.Position.Z) > PositionThreshold
            || Math.Abs(lastPlayerState.Yaw - self!.Entity!.Yaw) > PositionThreshold
            || Math.Abs(lastPlayerState.Pitch - self!.Entity!.Pitch) > PositionThreshold
            || lastPlayerState.OnGround != self!.Entity!.IsOnGround)
        {
            await UpdateServerPos();
        }
    }

    private async Task UpdateServerPos()
    {
        var packet = new SetPlayerPositionAndRotationPacket(
            self!.Entity!.Position.X,
            self!.Entity!.Position.Y,
            self!.Entity!.Position.Z,
            self!.Entity!.Yaw,
            self!.Entity!.Pitch,
            self!.Entity!.IsOnGround);

        lastPlayerState.X = self!.Entity!.Position.X;
        lastPlayerState.Y = self!.Entity!.Position.Y;
        lastPlayerState.Z = self!.Entity!.Position.Z;
        lastPlayerState.Yaw = self!.Entity!.Yaw;
        lastPlayerState.Pitch = self!.Entity!.Pitch;
        lastPlayerState.OnGround = self!.Entity!.IsOnGround;

        await Bot.Client.SendPacket(packet);

        _ = BotMoved.Dispatch(Bot);
    }

    private void OnSneakingChanged(PlayerPhysics sender, bool isSneaking)
    {
        var packet = new EntityActionPacket(
            playerPlugin!.Self!.Entity!.ServerId,
            isSneaking
                ? EntityActionPacket.EntityAction.StartSneaking
                : EntityActionPacket.EntityAction.StopSneaking,
            0);

        _ = Bot.Client.SendPacket(packet);
    }

    private void OnSprintingChanged(PlayerPhysics sender, bool isSprinting)
    {
        var packet = new EntityActionPacket(
            playerPlugin!.Self!.Entity!.ServerId,
            isSprinting
                ? EntityActionPacket.EntityAction.StartSprinting
                : EntityActionPacket.EntityAction.StopSprinting,
            0);

        _ = Bot.Client.SendPacket(packet);
    }

    private record PlayerState
    {
        public PlayerState(double x, double y, double z, float yaw, float pitch, bool ground)
        {
            X = x;
            Y = y;
            Z = z;
            Yaw = yaw;
            Pitch = pitch;
            OnGround = ground;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public bool OnGround { get; set; }

        public override string ToString()
        {
            return $"X={X} Y={Y} Z={Z} Yaw={Yaw} Pitch={Pitch} IsOnGround={OnGround}";
        }
    }

    private class LerpRotation
    {
        private readonly float pitchPerTick;
        private readonly MinecraftPlayer player;
        private readonly TaskCompletionSource task;
        private readonly float yawPerTick;
        private bool completed;
        private int remainingPitchTicks;
        private int remainingYawTicks;

        public LerpRotation(MinecraftPlayer player, float toYaw, float toPitch, float smoothness = RotationSmoothness)
        {
            this.player = player;

            var deltaYaw = toYaw - player.Entity!.Yaw;
            var deltaPitch = toPitch - player.Entity!.Pitch;

            yawPerTick = deltaYaw * smoothness;
            pitchPerTick = deltaPitch * smoothness;

            if (deltaYaw == 0)
            {
                yawPerTick = 1;
            }

            if (deltaPitch == 0)
            {
                pitchPerTick = 1;
            }

            var yawTicks = Math.Abs((int)(deltaYaw / yawPerTick));
            var pitchTicks = Math.Abs((int)(deltaPitch / pitchPerTick));

            task = new();
            remainingYawTicks = yawTicks;
            remainingPitchTicks = pitchTicks;
        }

        public void Tick()
        {
            if (completed)
            {
                return;
            }

            if (remainingPitchTicks > 0)
            {
                remainingPitchTicks--;
                player.Entity!.Pitch += pitchPerTick;
            }

            if (remainingYawTicks > 0)
            {
                remainingYawTicks--;
                player.Entity!.Yaw += yawPerTick;
            }

            if (remainingPitchTicks != 0 || remainingYawTicks != 0)
            {
                return;
            }

            completed = true;
            task.TrySetResult();
        }

        public void Cancel()
        {
            completed = true;
            task.TrySetCanceled();
        }

        public Task GetTask()
        {
            return task.Task;
        }
    }
}
