using MineSharp.Core.Common;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Effects;
using MineSharp.Core.Common.Entities;
using MineSharp.Core.Common.Entities.Attributes;
using MineSharp.Data;
using MineSharp.Physics.Input;
using MineSharp.Physics.Utils;
using MineSharp.World;
using MineSharp.World.Iterators;
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

    private readonly Input.Input input;
    private readonly Attribute speedAttribute;
    private readonly PlayerState state = new PlayerState();
    private readonly HashSet<BlockType> fluidOnEyes = new HashSet<BlockType>();

    public PlayerPhysics(MinecraftData data, MinecraftPlayer player, IWorld world, InputControls inputControls)
    {
        this.Data = data;
        this.Player = player;
        this.World = world;
        this.input = new Input.Input(inputControls);

        if (!this.Player.Entity!.Attributes.ContainsKey(PhysicsConst.ATTR_MOVEMENT_SPEED))
        {
            this.speedAttribute = new Attribute(
                PhysicsConst.ATTR_MOVEMENT_SPEED,
                PhysicsConst.DEFAULT_PLAYER_SPEED,
                Array.Empty<Modifier>());

            this.Player.Entity!.AddAttribute(this.speedAttribute);
        }
        else this.speedAttribute = this.Player.Entity!.GetAttribute(PhysicsConst.ATTR_MOVEMENT_SPEED)!;
    }

    public void OnTick()
    {
        Console.WriteLine($"Vel: {this.Player.Entity!.Velocity}, Pos: {this.Player.Entity.Position}");
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
        
        Console.WriteLine($"Vel: {this.Player.Entity!.Velocity}, Pos: {this.Player.Entity!.Position}");
    }

    private void MovementTick()
    {
        // TODO: Fix differences with java: MovementTick()
        
        this.state.IsCrouching = !PoseUtils.WouldPlayerCollideWithPose(this.Player, EntityPose.Crouching, this.World, this.Data)
                              && this.input.Controls.SneakingKeyDown // Swimming, Sleeping, Vehicle, Flying ; LocalPlayer.java:631
                              || PoseUtils.WouldPlayerCollideWithPose(this.Player, EntityPose.Standing, this.World, this.Data);
        
        var crouchSpeedFactor = (float)Math.Clamp(0.3f + 0.0, 0.0f, 1.0f); // Enchantment Soul Speed factor
        this.input.Tick(this.state.IsCrouching, crouchSpeedFactor); // not only forceCrouching, but also swimming LocalPlayer.java:633
        
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
        if (this.state.WasTouchingWater && this.input.Controls.SneakingKeyDown)
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
        if (this.input.Controls.JumpingKeyDown)
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

        var dx = this.Player.Entity!.Velocity.X * 0.98f;
        var dz = this.Player.Entity!.Velocity.Z * 0.98f;
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
        this.MoveRelative(moveVector, (float)frictionInfluencedSpeed, this.Player.Entity.Pitch);

        this.CheckClimbable();
        this.Move();

        var vel = this.Player.Entity.Velocity;
        var blockAtFeet = this.World.GetBlockAt((Position)this.Player.Entity.Position);
        if ((this.state.HorizontalCollision || this.input.Controls.JumpingKeyDown)
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
        if (y < 0.0 && blockAtFeet.Info.Type != BlockType.Scaffolding && this.input.Controls.SneakingKeyDown)
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
            this.Player.Entity!.Velocity = Vector3.Zero;
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
        this.state.VerticalCollision = vec.Y != vec3.Y;
        this.state.MinorHorizontalCollision = IsCollisionMinor(vec3);

        this.Player.Entity!.IsOnGround = this.state.VerticalCollision && vec.Y < 0;

        if (this.state.HorizontalCollision)
        {
            if (collidedX)
                this.Player.Entity.Velocity.X = 0.0;
            else
                this.Player.Entity.Velocity.Z = 0.0;
        }
    }

    private Vector3 Collide(Vector3 vec)
    {
        var aabb = this.Player.Entity!.GetBoundingBox();
        var vec3 = vec.LengthSquared() == 0.0 ? vec : CheckBoundingBoxCollisions(vec, aabb);

        var collidedX = Math.Abs(vec.X - vec3.X) > 1.0E-3;
        var collidedY = Math.Abs(vec.Y - vec3.Y) > 1.0E-3;
        var collidedZ = Math.Abs(vec.Z - vec3.Z) > 1.0E-3;
        var hitGround = this.Player.Entity!.IsOnGround || collidedY && vec.Y < 0.0;
        
        Console.WriteLine("CollideY: " + collidedY);

        if (hitGround && (collidedX || collidedZ))
        {
            var vec31 = CheckBoundingBoxCollisions(
                new Vector3(vec.X, PhysicsConst.MAX_UP_STEP, vec.Z), 
                aabb);
            var vec32 = CheckBoundingBoxCollisions(
                new Vector3(0.0, PhysicsConst.MAX_UP_STEP, 0.0), 
                CollisionUtils.ExpandBoundingBox(aabb, vec.X, 0, vec.Z));

            if (vec32.Y < PhysicsConst.MAX_UP_STEP)
            {
                var vec33 = CheckBoundingBoxCollisions(
                        new Vector3(vec.X, 0, vec.Z),
                        aabb.Clone().Offset(vec32.X, vec32.Y, vec32.Z))
                    .Plus(vec32);

                if (CollisionUtils.HorizontalDistanceSquared(vec33) > CollisionUtils.HorizontalDistanceSquared(vec31))
                    vec31 = vec33;
            }

            if (CollisionUtils.HorizontalDistanceSquared(vec31) > CollisionUtils.HorizontalDistanceSquared(vec3))
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

        var x = this.input.StrafeImpulse * cos * 0.98 - this.input.ForwardImpulse * sin * 0.98;
        var z = this.input.ForwardImpulse * cos * 0.98 - this.input.StrafeImpulse * sin * 0.98;

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

        var iterator = new BoundingBoxIterator(aabb);
        var shapes = iterator.Iterate()
            .Select(this.World.GetBlockAt)
            .Where(x => x.IsSolid())
            .Select(this.Data.BlockCollisionShapes.GetForBlock)
            .SelectMany(x => x)
            .ToArray();

        if (shapes.Length == 0)
            return direction;
        
        // check for collisions
        var mX = direction.X;
        var mY = direction.Y;
        var mZ = direction.Z;
        
        Console.WriteLine($"Collision Y:");
        if (mY != 0.0)
        {
            Console.WriteLine($"mY={mY} AABB={aabb}");
            mY = CalculateAxisOffset(CollisionUtils.Axis.YAxis, aabb, shapes, mY);
            if (mY != 0.0)
                aabb.Offset(0, mY, 0);
        }
        Console.WriteLine($"mY={mY} AABB={aabb}");
        Console.WriteLine("--- --- --- --- ---");

        var zDominant = Math.Abs(mX) < Math.Abs(mZ);
        if (zDominant && mZ != 0.0)
        {
            mZ = CalculateAxisOffset(CollisionUtils.Axis.ZAxis, aabb, shapes, mZ);
            if (mZ != 0.0)
                aabb.Offset(0, 0, mZ);
        }
        
        if (mX != 0.0)
        {
            mX = CalculateAxisOffset(CollisionUtils.Axis.XAxis, aabb, shapes, mX);
            if (mX != 0.0)
                aabb.Offset(0, 0, mX);
        }

        if (!zDominant && mZ != 0.0)
        {
            mZ = CalculateAxisOffset(CollisionUtils.Axis.ZAxis, aabb, shapes, mZ);
        }

        return new Vector3(mX, mY, mZ);
    }

    private double CalculateAxisOffset(CollisionUtils.Axis axis, AABB aabb, AABB[] shapes, double amount)
    {
        var value = amount;
        if (value == 0.0 || Math.Abs(amount) < 1.0E-7)
            return 0.0;

        foreach (var shape in shapes)
            value = CollisionUtils.CalculateMaxOffset(aabb, shape, axis, value);

        return value;
    }

    private void MaybeBackOffFromEdge(ref Vector3 vec)
    {
        // TODO Fix differences with java
        // Player.java:1054
        // && this.IsAboveGround()
        if (vec.Y > 0 || !this.input.Controls.SneakingKeyDown)
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
        var move = Vector3.Zero;
        if (length >= 1.0E-7D)
        {
            if (length > 1.0)
                vec.Normalized();
            vec.Scale(scale);

            var sin = MathF.Sin(pitch * (MathF.PI / 180.0F));
            var cos = MathF.Cos(pitch * (MathF.PI / 180.0F));
            move = new Vector3(vec.X * cos - vec.Z * sin, vec.Y, vec.Z * cos + vec.X * sin);
        }
        
        this.Player.Entity!.Velocity.Add(move);
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
            // TODO: Prob add sprinting attribute
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

        if (!this.CollidesWithSuffocatingBlock(pos))
        {
            return;
        }
        
        var nX = x - pos.X;
        var nZ = z - pos.Z;
        var diff = double.MaxValue;
        Vector3? direction = null;

        foreach (var axis in CollisionUtils.XZAxisVectors)
        {
            var coord = CollisionUtils.ChooseAxisCoordinate(axis, nX, 0.0, nZ);
            coord = CollisionUtils.IsPositiveAxisVector(axis)
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
