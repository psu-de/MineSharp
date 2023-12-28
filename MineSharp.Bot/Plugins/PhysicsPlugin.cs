
using MineSharp.Core.Common;
using MineSharp.Core.Common.Entities;
using MineSharp.Physics;
using MineSharp.Physics.Input;
using MineSharp.Protocol.Packets.Serverbound.Play;
using NLog;

namespace MineSharp.Bot.Plugins;

/// <summary>
/// Physics plugin simulates the player entity in the world.
/// It also allows for walking, jumping and crouching.
/// </summary>
public class PhysicsPlugin : Plugin 
{
    private const double POSITION_THRESHOLD = 0.01d;
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    
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
    
    private PlayerState lastPlayerState;
    private PlayerPhysics? physics;
    private PlayerPlugin? playerPlugin;
    private WorldPlugin? worldPlugin;

    private MinecraftPlayer? Self;

    /// <summary>
    /// Create a new PhysicsPlugin instance
    /// </summary>
    /// <param name="bot"></param>
    public PhysicsPlugin(MineSharpBot bot) : base(bot)
    {
        this.lastPlayerState = new PlayerState(0, 0, 0, 0, 0, false);
        this.InputControls = new InputControls();
    }

    /// <inheritdoc />
    protected override async Task Init()
    {
        this.playerPlugin = this.Bot.GetPlugin<PlayerPlugin>();
        this.worldPlugin = this.Bot.GetPlugin<WorldPlugin>();
        
        await this.playerPlugin.WaitForInitialization();
        await this.worldPlugin.WaitForChunks();

        this.Self = this.playerPlugin.Self;
        await this.UpdateServerPos();

        this.physics = new PlayerPhysics(this.Bot.Data, this.Self!, this.worldPlugin.World, this.InputControls);
        this.physics.OnCrouchingChanged += OnSneakingChanged;
        this.physics.OnSprintingChanged += OnSprintingChanged;
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
                this.physics!.Tick();
                await this.UpdateServerPositionIfNeeded();
                
                if (this.PhysicsTick != null)
                    await Task.Factory.FromAsync(
                        (callback, obj) => this.PhysicsTick.BeginInvoke(this.Bot, callback, obj),
                        this.PhysicsTick.EndInvoke,
                        null);
            } catch (Exception e)
            {
                Logger.Error(e.ToString());
            }
        });

        return Task.CompletedTask;
    }

    /// <summary>
    /// Forces the bots rotation to the given yaw and pitch (in degrees)
    /// </summary>
    /// <param name="yaw"></param>
    /// <param name="pitch"></param>
    public void ForceSetRotation(float yaw, float pitch)
    {
        this.Self!.Entity!.Yaw = yaw;
        this.Self!.Entity!.Pitch = pitch;
    }

    /// <summary>
    /// Forces the bot to look at given position
    /// </summary>
    /// <param name="position"></param>
    public void ForceLookAt(Position position)
    {
        var pos = new Vector3(0.5d, 0.5d, 0.5d).Plus(position);
        var r = pos.Minus(this.Self!.GetHeadPosition());
        var yaw = -Math.Atan2(r.X, r.Z) / Math.PI * 180;
        if (yaw < 0) yaw = 360 + yaw;
        var pitch = -Math.Asin(r.Y / r.Length()) / Math.PI * 180;
        this.ForceSetRotation((float)yaw, (float)pitch);
    }
    
    private async Task UpdateServerPositionIfNeeded()
    {
        if (Math.Abs(this.lastPlayerState.X - this.Self!.Entity!.Position.X) > POSITION_THRESHOLD 
            || Math.Abs(this.lastPlayerState.Y - this.Self!.Entity!.Position.Y) > POSITION_THRESHOLD 
            || Math.Abs(this.lastPlayerState.Z - this.Self!.Entity!.Position.Z) > POSITION_THRESHOLD 
            || Math.Abs(this.lastPlayerState.Yaw - this.Self!.Entity!.Yaw) > POSITION_THRESHOLD 
            || Math.Abs(this.lastPlayerState.Pitch - this.Self!.Entity!.Pitch) > POSITION_THRESHOLD 
            || this.lastPlayerState.OnGround != this.Self!.Entity!.IsOnGround)
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

        this.lastPlayerState.X = this.Self!.Entity!.Position.X;
        this.lastPlayerState.Y = this.Self!.Entity!.Position.Y;
        this.lastPlayerState.Z = this.Self!.Entity!.Position.Z;
        this.lastPlayerState.Yaw = this.Self!.Entity!.Yaw;
        this.lastPlayerState.Pitch = this.Self!.Entity!.Pitch;
        this.lastPlayerState.OnGround = this.Self!.Entity!.IsOnGround;

        await this.Bot.Client.SendPacket(packet);
        
        if (BotMoved != null)
            await Task.Factory.FromAsync(
                (callback, obj) => this.BotMoved.BeginInvoke(this.Bot, callback, obj),
                this.BotMoved.EndInvoke,
                null);
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
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public bool OnGround { get; set; }

        public PlayerState(double x, double y, double z, float yaw, float pitch, bool ground)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Yaw = yaw;
            this.Pitch = pitch;
            this.OnGround = ground;
        }

        public override string ToString() => $"X={this.X} Y={this.Y} Z={this.Z} Yaw={this.Yaw} Pitch={this.Pitch} IsOnGround={this.OnGround}";
    }
}