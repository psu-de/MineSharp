using MineSharp.Core.Common;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Effects;
using MineSharp.Core.Common.Entities;
using MineSharp.Core.Common.Entities.Attributes;
using MineSharp.Data;
using MineSharp.Physics.Components;
using MineSharp.Physics.Input;
using MineSharp.Physics.Utils;
using MineSharp.World;

using Attribute = MineSharp.Core.Common.Entities.Attributes.Attribute;

namespace MineSharp.Physics;

public class PlayerPhysics
{
    public delegate void PlayerPhysicsBooleanEvent(PlayerPhysics sender, bool crouching);

    public event PlayerPhysicsBooleanEvent? OnCrouchingChanged;
    public event PlayerPhysicsBooleanEvent? OnSprintingChanged;

    public readonly MinecraftData Data;
    public readonly MinecraftPlayer Player;
    public readonly IWorld World;

    private readonly FluidPhysicsComponent fluidPhysics;
    private readonly MovementInput movementInput;
    private readonly Attribute speedAttribute;
    private readonly Modifier sprintingModifier;
    private readonly PlayerState state = new PlayerState();
    
    private static readonly Vector3[] XZAxisVectors = new[] {
        Vector3.West, Vector3.East, Vector3.North, Vector3.South
    };

    public PlayerPhysics(MinecraftData data, MinecraftPlayer player, IWorld world, InputControls inputControls)
    {
        this.Data = data;
        this.Player = player;
        this.World = world;
        this.movementInput = new MovementInput(inputControls);
        this.fluidPhysics = new FluidPhysicsComponent(player, world, this.movementInput, this.state);

        if (!this.Player.Entity!.Attributes.ContainsKey(PhysicsConst.ATTR_MOVEMENT_SPEED))
        {
            this.speedAttribute = new Attribute(
                PhysicsConst.ATTR_MOVEMENT_SPEED,
                PhysicsConst.DEFAULT_PLAYER_SPEED,
                Array.Empty<Modifier>());

            this.Player.Entity!.AddAttribute(this.speedAttribute);
        }
        else this.speedAttribute = this.Player.Entity!.GetAttribute(PhysicsConst.ATTR_MOVEMENT_SPEED)!;
        this.sprintingModifier = new Modifier(
            UUID.Parse(PhysicsConst.SPRINTING_UUID),
            PhysicsConst.PLAYER_SPRINT_SPEED,
            ModifierOp.Multiply);
    }

    public void OnTick()
    {
        if (GameMode.Spectator == this.Player.GameMode)
        {
            this.Player.Entity!.IsOnGround = false;
        }
        
        // TODO: PlayerPhysics fix differences to java
        // Player.java:216 takeXpDelay
        //            :220 isSleeping()
        
        this.fluidPhysics.Tick();
        // updateSwimming()

        // updateUsingItem() LivingEntity.java:2241
        
        this.MovementTick();
    }

    private void MovementTick()
    {
        // TODO: Fix differences with java: MovementTick()

        var crouchedBefore = this.state.IsCrouching;
        this.state.IsCrouching = !PoseUtils.WouldPlayerCollideWithPose(this.Player, EntityPose.Crouching, this.World, this.Data)
                              && this.movementInput.Controls.SneakingKeyDown // Swimming, Sleeping, Vehicle, Flying ; LocalPlayer.java:631
                              || PoseUtils.WouldPlayerCollideWithPose(this.Player, EntityPose.Standing, this.World, this.Data);

        if (this.state.IsCrouching != crouchedBefore)
            Task.Run(() => this.OnCrouchingChanged?.Invoke(this, this.state.IsCrouching));

        var crouchSpeedFactor = (float)Math.Clamp(0.3f + 0.0, 0.0f, 1.0f); // Enchantment Soul Speed factor
        this.movementInput.Tick(this.state.IsCrouching, crouchSpeedFactor); // not only forceCrouching, but also swimming LocalPlayer.java:633
        
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
        // var startedFlying = false;
        
        // && is not flying && is not passenger
        // if (this.input.JumpedThisTick && !startedFlying && !this.IsOnClimbable())
        // {
            // TODO: Elytra flying
            // Try start falling
            // LocalPlayer.java:707
        // }

        // && !this.Player.IsFlying
        if (this.state.WasTouchingWater && this.movementInput.Controls.SneakingKeyDown)
        {
            this.Player.Entity!.Velocity.Add(PhysicsConst.WaterDrag);
        }
        
        // water vision
        
        // creative / spectator flying
        
        // jumping with vehicle

        if (this.state.NoJumpDelay > 0)
            this.state.NoJumpDelay--;
        
        this.TruncateVelocity(this.Player.Entity!.Velocity);

        // && !this.Player.IsFlying
        if (this.movementInput.Controls.JumpingKeyDown)
        {
            this.TryJumping();
        }
        else this.state.NoJumpDelay = 0;
        
        // UpdateFallFlying() (Elytra)
        var aabb = this.Player.Entity!.GetBoundingBox();
        // reset fall distance?
        
        // travelRidden() if player 
        this.Travel();
    }

    private void Travel()
    {
        // TODO: Swimming && !isPassenger()
        // TODO: Ability to fly

        var gravity = PhysicsConst.GRAVITY;
        var isFalling = this.Player.Entity!.Velocity.Y <= 0.0;
        if (isFalling && this.Player.Entity!.GetEffectLevel(EffectType.SlowFalling).HasValue)
            gravity = PhysicsConst.SLOW_FALLING_GRAVITY;

        var dx = this.movementInput.StrafeImpulse * 0.98f;
        var dz = this.movementInput.ForwardImpulse * 0.98f;
        var block = this.World.GetBlockAt((Position)this.Player.Entity!.Position);
        // LivingEntity.java:2015
        // && !abilities.canFly
        if (this.state.WasTouchingWater)
        {
            this.TravelWater(isFalling, dx, dz);
        } // else if in laval
        // else if fall flying (elytra)
        else
        {
            this.TravelNormally(gravity, dx, dz);
        }
    }

    private void TravelNormally(double gravity, double dx, double dz)
    {
        var moveVector = new Vector3(dx, 0, dz);
        var blockBelow = this.World.GetBlockAt(new Position(
            this.Player.Entity!.Position.X,
            this.Player.Entity!.Position.Y - 0.5f,
            this.Player.Entity!.Position.Z));
        var friction = PhysicsConst.GetBlockFriction(blockBelow.Info.Type);
        var speedFactor = this.Player.Entity.IsOnGround ? friction * 0.91f : 0.91f;

        var frictionInfluencedSpeed = this.Player.Entity.IsOnGround 
            ? this.speedAttribute.Value * (0.21600002f / Math.Pow(speedFactor, 3))
            : this.state.IsSprinting ? PhysicsConst.SPRINTING_FLYING_SPEED : PhysicsConst.FLYING_SPEED;
        this.MoveRelative(moveVector, (float)frictionInfluencedSpeed, this.Player.Entity.Yaw);

        this.CheckClimbable();
        this.Move();

        var vel = this.Player.Entity.Velocity;
        var blockAtFeet = this.World.GetBlockAt((Position)this.Player.Entity.Position);
        if ((this.state.HorizontalCollision || this.movementInput.Controls.JumpingKeyDown)
            && (WorldUtils.IsOnClimbable(this.Player, this.World, ref this.state.LastClimbablePosition) || blockAtFeet.Info.Type == BlockType.PowderSnow))
        {
            vel = new Vector3(vel.X, 0.2, vel.Z);
        }

        var velY = vel.Y;
        var levitation = this.Player.Entity!.GetEffectLevel(EffectType.Levitation);
        if (levitation.HasValue)
            velY += (0.05d * (levitation.Value + 1) - velY) * 0.2d;
        else if (!this.World.IsChunkLoaded(this.World.ToChunkCoordinates((Position)this.Player.Entity.Position)))
        {
            if (this.Player.Entity.Position.Y > this.World.MinY)
                velY = 0.1d;
            else velY = 0.0;
        }
        else
            velY -= gravity;

        this.Player.Entity.Velocity.X = vel.X * speedFactor;
        this.Player.Entity.Velocity.Y = velY * 0.98f;
        this.Player.Entity.Velocity.Z = vel.Z * speedFactor;
    }

    private void CheckClimbable()
    {
        if (!WorldUtils.IsOnClimbable(this.Player, this.World, ref this.state.LastClimbablePosition))
            return;

        var x = Math.Clamp(this.Player.Entity!.Velocity.X, -PhysicsConst.CLIMBING_SPEED, PhysicsConst.CLIMBING_SPEED);
        var z = Math.Clamp(this.Player.Entity!.Velocity.Z, -PhysicsConst.CLIMBING_SPEED, PhysicsConst.CLIMBING_SPEED);
        var y = Math.Max(this.Player.Entity!.Velocity.Y, -PhysicsConst.CLIMBING_SPEED);

        var blockAtFeet = this.World.GetBlockAt((Position)this.Player.Entity.Position);
        // && !abilities.flying
        if (y < 0.0 && blockAtFeet.Info.Type != BlockType.Scaffolding && this.movementInput.Controls.SneakingKeyDown)
            y = 0.0;

        this.Player.Entity!.Velocity.X = x;
        this.Player.Entity!.Velocity.Y = y;
        this.Player.Entity!.Velocity.Z = z;
    }

    private void TravelWater(bool isFalling, double dx, double dz)
    {
        var lastY = this.Player.Entity!.Position.Y;
        var waterFactor = this.state.IsSprinting ? 0.9f : 0.8f;
        var speedFactor = 0.02f;
        var depthStrider = 0.0f; // TODO: get Depth strider level
        depthStrider = MathF.Max(3.0f, depthStrider);

        if (!this.Player.Entity!.IsOnGround)
            depthStrider *= 0.5f;

        if (depthStrider > 0.0)
        {
            waterFactor += (0.54600006F - waterFactor) * depthStrider / 3.0F;
            speedFactor += ((float)this.speedAttribute.Value - speedFactor) * depthStrider / 3.0F;
        }

        if (this.Player.Entity!.GetEffectLevel(EffectType.DolphinsGrace).HasValue)
            waterFactor = 0.96f;
        
        var moveVector = new Vector3(dx, 0, dz);
        this.MoveRelative(moveVector, speedFactor, this.Player.Entity!.Pitch);
        this.Move();
        
        // TODO: Finish water movement
    }

    private void Move()
    {
        var vec = this.Player.Entity!.Velocity.Clone();

        if (this.state.StuckSpeedMultiplier.LengthSquared() > 1.0E-7D)
        {
            vec.Multiply(this.state.StuckSpeedMultiplier);
            this.state.StuckSpeedMultiplier = Vector3.Zero;
            this.Player.Entity!.Velocity.X = 0;
            this.Player.Entity!.Velocity.Y = 0;
            this.Player.Entity!.Velocity.Z = 0;
        }

        this.MaybeBackOffFromEdge(ref vec);
        var vec3 = this.Collide(vec);

        var length = vec3.LengthSquared();
        if (length > 1.0E-7D)
        {
            // reset fall distance
            this.Player.Entity!.Position.Add(vec3);
        }

        var collidedX = Math.Abs(vec.X - vec3.X) > 1.0E-5F;
        var collidedZ = Math.Abs(vec.Z - vec3.Z) > 1.0E-5F;
        this.state.HorizontalCollision = collidedX || collidedZ;
        this.state.VerticalCollision = Math.Abs(vec.Y - vec3.Y) > 1.0E-5F;
        this.state.MinorHorizontalCollision = IsCollisionMinor(vec3);

        this.Player.Entity!.IsOnGround = this.state.VerticalCollision && vec.Y < 0;

        if (this.state.HorizontalCollision)
        {
            if (collidedX)
                this.Player.Entity.Velocity.X = 0.0;
            else
                this.Player.Entity.Velocity.Z = 0.0;
        }
        else this.state.MinorHorizontalCollision = false;

        if (this.state.VerticalCollision)
        {
            this.Player.Entity.Velocity.Y = 0.0;
        }
    }

    private Vector3 Collide(Vector3 vec)
    {
        var aabb = this.Player.Entity!.GetBoundingBox();
        var vec3 = vec.LengthSquared() == 0.0 ? vec : CheckBoundingBoxCollisions(vec, aabb);

        var collidedX = Math.Abs(vec.X - vec3.X) > 1.0E-5;
        var collidedY = Math.Abs(vec.Y - vec3.Y) > 1.0E-5;
        var collidedZ = Math.Abs(vec.Z - vec3.Z) > 1.0E-5;
        var hitGround = this.Player.Entity!.IsOnGround || collidedY && vec.Y < 0.0;

        if (hitGround && (collidedX || collidedZ))
        {
            var vec31 = CheckBoundingBoxCollisions(
                new Vector3(vec.X, PhysicsConst.MAX_UP_STEP, vec.Z), 
                aabb);
            var vec32 = CheckBoundingBoxCollisions(
                new Vector3(0.0, PhysicsConst.MAX_UP_STEP, 0.0), 
                aabb.ExpandBoundingBox(vec.X, 0, vec.Z));

            if (vec32.Y < PhysicsConst.MAX_UP_STEP)
            {
                var vec33 = CheckBoundingBoxCollisions(
                        new Vector3(vec.X, 0, vec.Z),
                        aabb.Clone().Offset(vec32.X, vec32.Y, vec32.Z))
                    .Plus(vec32);

                if (vec33.HorizontalLengthSquared() > vec31.HorizontalLengthSquared())
                    vec31 = vec33;
            }

            if (vec31.HorizontalLengthSquared() > vec3.HorizontalLengthSquared())
            {
                vec31.Add(
                    CheckBoundingBoxCollisions(
                        new Vector3(0.0, -vec31.Y + vec.Y, 0.0),
                        aabb.Clone().Offset(vec31.X, vec31.Y, vec31.Z)));
                return vec31;
            }
        }

        return vec3;
    }

    private bool IsCollisionMinor(Vector3 vec)
    {
        var pitchRadians = this.Player.Entity!.Pitch * (Math.PI / 180.0);
        var sin = Math.Sin(pitchRadians);
        var cos = Math.Cos(pitchRadians);

        var x = this.movementInput.StrafeImpulse * cos * 0.98 - this.movementInput.ForwardImpulse * sin * 0.98;
        var z = this.movementInput.ForwardImpulse * cos * 0.98 - this.movementInput.StrafeImpulse * sin * 0.98;

        var s1 = Math.Pow(x, 2) + Math.Pow(z, 2);
        var s2 = Math.Pow(vec.X, 2) + Math.Pow(vec.Z, 2);

        if (s1 < 1.0E-5F || s2 < 1.0E-5F)
            return false;

        double d6 = x * vec.X + z * vec.Z;
        double d7 = Math.Acos(d6 / Math.Sqrt(s1 * s2));
        return d7 < 0.13962634F;
    }
    
    private Vector3 CheckBoundingBoxCollisions(Vector3 direction, AABB aabb)
    {
        // TODO: Entity collision boxes
        // TODO: World border collision
        
        var shapes = WorldUtils.GetWorldBoundingBoxes(aabb, this.World, this.Data);

        if (shapes.Length == 0)
            return direction;
        
        // check for collisions
        var mX = direction.X;
        var mY = direction.Y;
        var mZ = direction.Z;
        
        if (mY != 0.0)
        {
            mY = CalculateAxisOffset(Axis.Y, aabb, shapes, mY);
            if (mY != 0.0)
                aabb.Offset(0, mY, 0);
        }

        var zDominant = Math.Abs(mX) < Math.Abs(mZ);
        if (zDominant && mZ != 0.0)
        {
            mZ = CalculateAxisOffset(Axis.Z, aabb, shapes, mZ);
            if (mZ != 0.0)
                aabb.Offset(0, 0, mZ);
        }
        
        if (mX != 0.0)
        {
            mX = CalculateAxisOffset(Axis.X, aabb, shapes, mX);
            if (mX != 0.0)
                aabb.Offset(0, 0, mX);
        }

        if (!zDominant && mZ != 0.0)
        {
            mZ = CalculateAxisOffset(Axis.Z, aabb, shapes, mZ);
        }

        return new Vector3(mX, mY, mZ);
    }

    private double CalculateAxisOffset(Axis axis, AABB aabb, AABB[] shapes, double displacement)
    {
        var value = displacement;
        if (value == 0.0 || Math.Abs(displacement) < 1.0E-7)
            return 0.0;

        foreach (var shape in shapes)
            value = shape.CalculateMaxOffset(aabb, axis, value);

        return value;
    }

    private void MaybeBackOffFromEdge(ref Vector3 vec)
    {
        // TODO Fix differences with java
        // Player.java:1054
        // && this.IsAboveGround()
        if (vec.Y > 0 || !this.movementInput.Controls.SneakingKeyDown)
            return;

        var x = vec.X;
        var z = vec.Z;
        var entity = this.Player.Entity!;
        
        // TODO: Check for entity collisions, not only block collisions
        const double increment = 0.05d;
        while (x != 0.0D && !WorldUtils.CollidesWithWorld(entity.GetBoundingBox().Offset(x, -1.0, 0.0), this.World, this.Data))
        {
            x = x switch {
                < increment and >= -increment => 0.0,
                > 0.0 => x - increment,
                _ => x + increment
            };
        }

        while (z != 0.0D && !WorldUtils.CollidesWithWorld(entity.GetBoundingBox().Offset(0.0, -1.0, z), this.World, this.Data))
        {
            z = z switch {
                < increment and >= -increment => 0.0,
                > 0.0 => z - increment,
                _ => z + increment
            };
        }
        
        while(x != 0.0D && z != 0.0D && !WorldUtils.CollidesWithWorld(entity.GetBoundingBox().Offset(x, -1.0, z), this.World, this.Data)) {
            x = x switch {
                < increment and >= -increment => 0.0,
                > 0.0 => x - increment,
                _ => x + increment
            };

            z = z switch {
                < increment and >= -increment => 0.0,
                > 0.0 => z - increment,
                _ => z + increment
            };
        }

        vec.X = x;
        vec.Z = z;
    }

    private void MoveRelative(Vector3 vec, float scale, float pitch)
    {
        var length = vec.LengthSquared();
        if (length < 1.0E-7D) 
            return;
        
        if (length > 1.0)
            vec.Normalized();
        
        vec.Scale(scale);

        var sin = MathF.Sin(pitch * (MathF.PI / 180.0F));
        var cos = MathF.Cos(pitch * (MathF.PI / 180.0F));
        this.Player.Entity!.Velocity.X += vec.X * cos - vec.Z * sin;
        this.Player.Entity!.Velocity.Y += vec.Y;
        this.Player.Entity!.Velocity.Z += vec.Z * cos + vec.X * sin;
    }

    private void TryJumping()
    {
        var fluidHeight = this.state.LavaHeight > 0.0
            ? this.state.LavaHeight
            : this.state.WaterHeight;

        var isInWater = this.state.WasTouchingWater && fluidHeight > 0.0;
        if (!isInWater || this.Player.Entity!.IsOnGround && fluidHeight <= PhysicsConst.FLUID_JUMP_THRESHOLD)
        {
            if (this.state.LavaHeight <= 0.0 || this.Player.Entity!.IsOnGround && fluidHeight <= PhysicsConst.FLUID_JUMP_THRESHOLD)
            {
                if ((this.Player.Entity!.IsOnGround || isInWater && fluidHeight <= PhysicsConst.FLUID_JUMP_THRESHOLD) && this.state.NoJumpDelay == 0)
                {
                    this.JumpFromGround();
                    this.state.NoJumpDelay = PhysicsConst.JUMP_DELAY;
                }
            }
            else
            {
                this.JumpInFluid();
            }
        }
        else
        {
            this.JumpInFluid();
        }
    }

    private void JumpFromGround()
    {
        var jumpBoostFactor = 0.1d * (this.Player.Entity!.GetEffectLevel(EffectType.JumpBoost) + 1) ?? 0;
        
        this.Player.Entity!.Velocity.Y = 0.42d * WorldUtils.GetBlockJumpFactor((Position)this.Player.Entity!.Position, this.World) + jumpBoostFactor;
        if (!this.state.IsSprinting)
            return;

        var pitch = this.Player.Entity!.Pitch * (MathF.PI / 180.0f);
        this.Player.Entity!.Velocity.X -= 0.2F * MathF.Sin(pitch);
        this.Player.Entity!.Velocity.Z += 0.2F * MathF.Cos(pitch);
    }

    private void JumpInFluid()
    {
        this.Player.Entity!.Velocity.Y += PhysicsConst.FLUID_JUMP_FACTOR;
    }

    private void UpdateSprinting()
    {
        var hasSprintingImpulse = this.movementInput.HasSprintingImpulse(this.state.WasUnderwater);
        
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

        if ((!this.state.WasTouchingWater || this.state.WasUnderwater) && canStartSprinting && this.movementInput.Controls.SprintingKeyDown)
        {
            // TODO: Prob add sprinting attribute
            this.speedAttribute.AddModifier(this.sprintingModifier);
            this.state.IsSprinting = true;
            Task.Run(() => this.OnSprintingChanged?.Invoke(this, this.state.IsSprinting));
        }

        if (this.state.IsSprinting)
        {
            var cannotSprint = !this.movementInput.HasForwardImpulse(); // || hasEnoughFoodToStartSprinting()
            var stopSprinting = cannotSprint 
                                || this.state.HorizontalCollision && !this.state.MinorHorizontalCollision 
                                || this.state.WasTouchingWater && !this.state.WasUnderwater;
            
            // isSwimming() LocalPlayer.java:676
            // else
            if (stopSprinting)
            {
                this.speedAttribute.RemoveModifier(this.sprintingModifier.UUID);
                this.state.IsSprinting = false;
                Task.Run(() => this.OnSprintingChanged?.Invoke(this, this.state.IsSprinting));
            }
        }
    }
    
    private void PushTowardsClosestSpace(double x, double z)
    {
        var pos = new Position(x, this.Player.Entity!.Position.Y, z);

        if (!this.CollidesWithSuffocatingBlock(pos))
        {
            return;
        }
        
        var nX = x - pos.X;
        var nZ = z - pos.Z;
        var diff = double.MaxValue;
        Vector3? direction = null;

        foreach (var axis in XZAxisVectors)
        {
            var coord = axis.ChooseValueForAxis(nX, 0.0, nZ);
            coord = axis.IsPositiveAxisVector()
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

        return WorldUtils.CollidesWithWorld(bb, this.World, this.Data);
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
