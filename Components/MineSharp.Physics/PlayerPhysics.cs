using MineSharp.Core.Common;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Entities;
using MineSharp.Data;
using MineSharp.Physics.Input;
using MineSharp.Physics.Utils;
using MineSharp.World;

namespace MineSharp.Physics;

public class PlayerPhysics(MinecraftData data, MinecraftPlayer player, IWorld world, InputControls inputControls)
{
    public delegate void PlayerPhysicsBooleanEvent(PlayerPhysics sender, bool crouching);

    public event PlayerPhysicsBooleanEvent? OnCrouchingChanged;
    public event PlayerPhysicsBooleanEvent? OnSprintingChanged;
    
    public readonly MinecraftData Data = data;
    public readonly MinecraftPlayer Player = player;
    public readonly IWorld World = world;
    
    private readonly Input.Input input = new Input.Input(inputControls);
    private PlayerState state = new PlayerState();
    
    private HashSet<BlockType> fluidOnEyes = new HashSet<BlockType>();

    public void OnTick()
    {
        if (GameMode.Spectator == this.Player.GameMode)
        {
            this.Player.Entity!.IsOnGround = false;
        }
        
        // TODO: PlayerPhysics fix differences to java
        // Player.java:216 takeXpDelay
        //            :220 isSleeping()

        this.CheckIfUnderwater();
        
        // Entity.java:421 vehicle movement

        this.DoFluidMovements();
        this.UpdateOnEyeFluids();
        // updateSwimming()

        // updateUsingItem() LivingEntity.java:2241
        
        this.MovementTick();
        
        this.Player.Entity!.Position.Add(
            this.Player.Entity!.Velocity);
    }

    private void MovementTick()
    {
        // TODO: Fix differences with java: MovementTick()
        this.TruncateVelocity(this.Player.Entity!.Velocity);

        bool forceCrouching = !PoseUtils.WouldPlayerCollideWithPose(this.Player, EntityPose.Crouching, this.World, this.Data)
                              && this.input.Controls.SneakingKeyDown // Swimming, Sleeping, Vehicle, Flying ; LocalPlayer.java:631
                              || PoseUtils.WouldPlayerCollideWithPose(this.Player, EntityPose.Standing, this.World, this.Data);
        
        var crouchSpeedFactor = (float)Math.Clamp(0.3f + 0.0, 0.0f, 1.0f); // Enchantment Soul Speed factor
        this.input.Tick(forceCrouching, crouchSpeedFactor); // not only forceCrouching, but also swimming LocalPlayer.java:633
        
        // Is using item LocalPlayer.java:635
        
        var bb = this.Player.Entity!.GetBoundingBox();
        var x = this.Player.Entity.Position.X;
        var z = this.Player.Entity.Position.Z;
        var w = bb.Width;
        this.PushTowardsClosestSpace(x - w * 0.35d, z + w * 0.35d);
        this.PushTowardsClosestSpace(x - w * 0.35d, z - w * 0.35d);
        this.PushTowardsClosestSpace(x + w * 0.35d, z - w * 0.35d);
        this.PushTowardsClosestSpace(x + w * 0.35d, z + w * 0.35d);

        this.UpdateSprinting();
        
        // ability to fly for creative and spectator
        var startedFlying = false;
        
        // && is not flying && is not passenger
        if (this.input.JumpedThisTick && !startedFlying && !this.IsOnClimbable())
        {
            // TODO: Elytra flying
            // Try start falling
            // LocalPlayer.java:707
        }
    }

    private bool IsOnClimbable()
    {
        if (player.GameMode == GameMode.Spectator)
            return false;

        var position = (Position)this.Player.Entity!.Position;
        var blockAtFeet = this.World.GetBlockAt(position);

        // TODO: Change once tags are implemented
        if (blockAtFeet.Info.Type is BlockType.GlowLichen or BlockType.Ladder or BlockType.Scaffolding
            or BlockType.TwistingVines or BlockType.Vine or BlockType.CaveVines or BlockType.WeepingVines)
        {
            this.state.LastClimbablePosition = position;
            return true;
        }
        if (!blockAtFeet.Info.Name.StartsWith("trapdoor"))
            return false;

        var blockBelowPos = (Position)Vector3.Down.Plus(blockAtFeet.Position);
        var blockBelow = this.World.GetBlockAt(blockBelowPos);
        if (!blockBelow.GetProperty<bool>("open")
            || blockBelow.GetProperty<string>("facing") != blockAtFeet.GetProperty<string>("facing"))
            return false;

        this.state.LastClimbablePosition = position;
        return true;
    }

    private void UpdateSprinting()
    {
        var hasSprintingImpulse = this.input.HasSprintingImpulse(this.state.WasUnderwater);
        
        // TODO: fix differences with java
        // LocalPlayer.java:975 canStartSprinting()
        // hasEnoughFoodToStartSprinting() && !isUsingItem()
        // + effects + vehicle
        var canStartSprinting = !this.state.IsSprinting && hasSprintingImpulse;
        // var onGround = this.Player.Entity!.IsOnGround; // isPassenger() -> getVehicle.IsOnGround
        // var flag = !this.input.Controls.SneakingKeyDown && !hasSprintingImpulse;
        //
        // if ((onGround || this.state.WasUnderwater) && flag && canStartSprinting)
        // {
        //     
        // }

        if ((!this.state.WasTouchingWater || this.state.WasUnderwater) && canStartSprinting && this.input.Controls.SprintingKeyDown)
        {
            this.state.IsSprinting = true;
            Task.Run(() => this.OnSprintingChanged?.Invoke(this, this.state.IsSprinting));
        }

        if (this.state.IsSprinting)
        {
            var cannotSprint = !this.input.HasForwardImpulse(); // || hasEnoughFoodToStartSprinting()
            var stopSprinting = cannotSprint 
                                || this.state.HorizontalCollision 
                                || !this.state.MinorHorizontalCollision 
                                || this.state.WasTouchingWater && !this.state.WasUnderwater;
            
            // isSwimming() LocalPlayer.java:676
            // else
            if (stopSprinting)
            {
                this.state.IsSprinting = false;
                Task.Run(() => this.OnSprintingChanged?.Invoke(this, this.state.IsSprinting));
            }
        }
    }
    
    private void PushTowardsClosestSpace(double x, double z)
    {
        var pos = new Position(x, this.Player.Entity!.Position.Y, z);

        if (!CollidesWithSuffocatingBlock(pos))
        {
            return;
        }
        
        var nX = x - pos.X;
        var nZ = z - pos.Z;
        var diff = double.MaxValue;
        Vector3? direction = null;

        foreach (var axis in VectorUtils.XZAxisVectors)
        {
            var coord = VectorUtils.ChooseAxisCoordinate(axis, nX, 0.0, nZ);
            coord = VectorUtils.IsPositiveAxisVector(axis)
                ? 1.0 - coord
                : coord;

            if (coord >= diff || this.CollidesWithSuffocatingBlock((Position)axis.Plus(pos)))
                continue;

            direction = axis;
            diff = coord;
        }

        if (null == direction)
            return;

        if (direction.X != 0.0)
            this.Player.Entity!.Velocity.X = 0.1d * direction.X;
        else
            this.Player.Entity!.Velocity.Z = 0.1 * direction.Z;
    }

    private bool CollidesWithSuffocatingBlock(Position position)
    {
        var bb = this.Player.Entity!.GetBoundingBox();
        bb.MinX = position.X;
        bb.MinZ = position.Z;
        bb.MaxX = position.X + 1.0f;
        bb.MaxZ = position.Z + 1.0f;
        bb.Contract(1.0E-7D, 1.0E-7D, 1.0E-7D);

        return CollisionUtils.CollidesWithWorld(bb, this.World, this.Data);
    }

    private void CheckIfUnderwater()
    {
        this.state.WasUnderwater = this.fluidOnEyes.Contains(BlockType.Water);
    }

    private bool DoFluidMovements()
    {
        this.state.WaterHeight = 0.0d;
        this.state.LavaHeight = 0.0d;
        
        // TODO: PlayerPhysics fix differences to java
        // Entity.java:1205 vehicle movement (boat)
        
        if (this.DoFluidPushing(BlockType.Water, 0.014d, out var waterHeight))
        {
            this.state.WasTouchingWater = true;
        }
        else this.state.WasTouchingWater = false;

        var lavaFactor = this.Player.Dimension == Dimension.Nether
            ? 0.007D
            : 0.0023333333333333335D;

        var wasInLava = this.DoFluidPushing(BlockType.Lava, lavaFactor, out var lavaHeight);

        this.state.WaterHeight = waterHeight;
        this.state.LavaHeight = lavaHeight;
        return wasInLava || this.state.WasTouchingWater;
    }

    private bool DoFluidPushing(BlockType type, double factor, out double height)
    {
        var aabb = this.Player.Entity!
            .GetBoundingBox()
            .Contract(0.001d, 0.001d, 0.001d);

        var fromX = (int)Math.Floor(aabb.MinX);
        var toX = (int)Math.Ceiling(aabb.MaxX);
        var fromY = (int)Math.Floor(aabb.MinY);
        var toY = (int)Math.Ceiling(aabb.MaxY);
        var fromZ = (int)Math.Floor(aabb.MinZ);
        var toZ = (int)Math.Ceiling(aabb.MaxZ);
        var d0 = 0.0d;
        var result = false;
        var vel = Vector3.Zero;
        var k1 = 0;

        var pos = new MutablePosition(0, 0 ,0);
        for (int x = fromX; x < toX; ++x) // TODO: Implement world iterators, that would be nicer
        {
            for (int y = fromY; y < toY; ++y)
            {
                for (int z = fromZ; z < toZ; ++z)
                {
                    pos.Set(x, y, z);
                    var block = this.World.GetBlockAt(pos);

                    if (block.Info.Type != type)
                        continue;

                    var fHeight = y + FluidUtils.GetFluidHeight(this.World, block);
                    if (fHeight < aabb.MinY)
                        continue;

                    result = true;
                    d0 = Math.Max(fHeight - aabb.MinY, d0);
                    var flow = FluidUtils.GetFlow(this.World, block);
                    Console.WriteLine($"Flow of {pos}: " + flow);
                    
                    if (d0 < 0.4d) 
                        flow.Scale(d0);
                    
                    vel.Add(flow);
                    k1++;
                }
            }
        }
        
        height = d0;

        if (vel.LengthSquared() == 0)
            return result;

        if (k1 > 0)
            vel.Scale(1.0d / k1);
        
        vel.Scale(factor);

        var delta = this.Player.Entity!.Velocity;
        if (Math.Abs(delta.X) < PhysicsConst.VELOCITY_THRESHOLD
            && Math.Abs(delta.Z) < PhysicsConst.VELOCITY_THRESHOLD
            && vel.Length() < PhysicsConst.VELOCITY_SCALE)
        {
            vel.Normalize();
            vel.Scale(PhysicsConst.VELOCITY_SCALE);
        }
        
        this.Player.Entity!.Velocity.Add(vel);
        
        return result;
    }

    private void UpdateOnEyeFluids()
    {
        this.fluidOnEyes.Clear();
        var eyeHeight = this.Player.GetEyeY() - 0.11111111d;
        
        // TODO: UpdateOnEyeFluids(): Differences with java
        // Entity.java:1230 updateFluidOnEyes()
        // this.getVehicle() instanceof Boat

        var pos = new Position(
            this.Player.Entity!.Position.X, 
            eyeHeight,
            this.Player.Entity!.Position.Z);
        var block = this.World.GetBlockAt(pos);

        if (!block.IsFluid())
            return;

        var fluidHeight = block.Position.Y + FluidUtils.GetFluidHeight(this.World, block);

        if (fluidHeight > eyeHeight)
            this.fluidOnEyes.Add(block.Info.Type);
    }

    private void TruncateVelocity(Vector3 vec)
    {
        if (Math.Abs(vec.X) < PhysicsConst.VELOCITY_THRESHOLD)
            vec.X = 0;
        if (Math.Abs(vec.Y) < PhysicsConst.VELOCITY_THRESHOLD)
            vec.Y = 0;
        if (Math.Abs(vec.Z) < PhysicsConst.VELOCITY_THRESHOLD)
            vec.Z = 0;
    }
}
