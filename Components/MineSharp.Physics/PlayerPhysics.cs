using MineSharp.Core.Common;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Effects;
using MineSharp.Core.Common.Entities;
using MineSharp.Core.Common.Entities.Attributes;
using MineSharp.Core.Events;
using MineSharp.Core.Geometry;
using MineSharp.Data;
using MineSharp.Physics.Components;
using MineSharp.Physics.Input;
using MineSharp.Physics.Utils;
using MineSharp.World;
using Attribute = MineSharp.Core.Common.Entities.Attributes.Attribute;

namespace MineSharp.Physics;

/// <summary>
///     Simulate a Minecraft player in a minecraft world
/// </summary>
public class PlayerPhysics
{
    private static readonly Vector3[] XzAxisVectors = { Vector3.West, Vector3.East, Vector3.North, Vector3.South };

    /// <summary>
    ///     The minecraft data used for this instance
    /// </summary>
    public readonly MinecraftData Data;

    private readonly FluidPhysicsComponent fluidPhysics;
    private readonly MovementInput movementInput;

    /// <summary>
    ///     The player to simulate
    /// </summary>
    public readonly MinecraftPlayer Player;

    private readonly MutableVector3 playerPosition;
    private readonly MutableVector3 playerVelocity;
    private readonly Attribute speedAttribute;
    private readonly Modifier sprintingModifier;

    /// <summary>
    ///     The physics state of this PlayerPhysics instance
    /// </summary>
    public readonly PlayerState State = new();

    /// <summary>
    ///     The world
    /// </summary>
    public readonly IWorld World;

    /// <summary>
    ///     Create a new PlayerPhysics instance
    /// </summary>
    /// <param name="data"></param>
    /// <param name="player"></param>
    /// <param name="world"></param>
    /// <param name="inputControls"></param>
    public PlayerPhysics(MinecraftData data, MinecraftPlayer player, IWorld world, InputControls inputControls)
    {
        Data = data;
        Player = player;
        World = world;
        movementInput = new(inputControls);
        fluidPhysics = new(player, world, movementInput, State);

        if (!Player.Entity!.Attributes.ContainsKey(PhysicsConst.AttrMovementSpeed))
        {
            speedAttribute = new(
                PhysicsConst.AttrMovementSpeed,
                PhysicsConst.DefaultPlayerSpeed,
                Array.Empty<Modifier>());

            Player.Entity!.AddAttribute(speedAttribute);
        }
        else
        {
            speedAttribute = Player.Entity!.GetAttribute(PhysicsConst.AttrMovementSpeed)!;
        }

        sprintingModifier = new(
            Uuid.Parse(PhysicsConst.SprintingUuid),
            PhysicsConst.PlayerSprintSpeed,
            ModifierOp.Multiply);

        playerVelocity = Player.Entity.Velocity as MutableVector3 ??
            throw new ArgumentException($"player velocity is not a {nameof(MutableVector3)}");

        playerPosition = Player.Entity.Position as MutableVector3 ??
            throw new ArgumentException($"entity position is not a {nameof(MutableVector3)}");
    }

    /// <summary>
    ///     Fired when the player starts or stops crouching
    /// </summary>
    public AsyncEvent<PlayerPhysics, bool> OnCrouchingChanged = new();

    /// <summary>
    ///     Fired when the player starts or stops sprinting
    /// </summary>
    public AsyncEvent<PlayerPhysics, bool> OnSprintingChanged = new();

    /// <summary>
    ///     Simulate a single tick
    /// </summary>
    public void Tick()
    {
        if (GameMode.Spectator == Player.GameMode)
        {
            Player.Entity!.IsOnGround = false;
        }

        // TODO: The Vanilla client does some things even when the Chunk is not loaded
        if (!World.IsChunkLoaded(
            World.ToChunkCoordinates((Position)Player.Entity!.Position)))
        {
            return;
        }

        // TODO: PlayerPhysics fix differences to java
        // Player.java:216 takeXpDelay
        //            :220 isSleeping()

        fluidPhysics.Tick();

        // updateSwimming()

        // updateUsingItem() LivingEntity.java:2241

        MovementTick();
    }

    private void MovementTick()
    {
        // TODO: Fix differences with java: MovementTick()

        var crouchedBefore = State.IsCrouching;

        var collidesCrouching = PoseUtils.WouldPlayerCollideWithPose(Player, EntityPose.Crouching, World, Data);
        var collidesStanding = PoseUtils.WouldPlayerCollideWithPose(Player, EntityPose.Standing, World, Data);

        // Swimming, Sleeping, Vehicle, Flying ; LocalPlayer.java:631
        State.IsCrouching = (!collidesCrouching
            && movementInput.Controls.SneakingKeyDown) || collidesStanding;

        if (State.IsCrouching != crouchedBefore)
        {
            OnCrouchingChanged.Dispatch(this, State.IsCrouching);
        }

        var crouchSpeedFactor = (float)Math.Clamp(0.3f + 0.0, 0.0f, 1.0f); // Enchantment Soul Speed factor
        movementInput.Tick(State.IsCrouching,
                           crouchSpeedFactor); // not only forceCrouching, but also swimming LocalPlayer.java:633

        // Is using item LocalPlayer.java:635

        var bb = Player.Entity!.GetBoundingBox();
        var x = Player.Entity.Position.X;
        var z = Player.Entity.Position.Z;
        var w = bb.Width;
        PushTowardsClosestSpace(x - (w * 0.35d), z + (w * 0.35d));
        PushTowardsClosestSpace(x - (w * 0.35d), z - (w * 0.35d));
        PushTowardsClosestSpace(x + (w * 0.35d), z - (w * 0.35d));
        PushTowardsClosestSpace(x + (w * 0.35d), z + (w * 0.35d));

        UpdateSprinting();

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
        if (State.WasTouchingWater && movementInput.Controls.SneakingKeyDown)
        {
            playerVelocity.Add(PhysicsConst.WaterDrag);
        }

        // water vision

        // creative / spectator flying

        // jumping with vehicle

        if (State.NoJumpDelay > 0)
        {
            State.NoJumpDelay--;
        }

        TruncateVelocity(playerVelocity);

        // && !this.Player.IsFlying
        if (movementInput.Controls.JumpingKeyDown)
        {
            TryJumping();
        }
        else
        {
            State.NoJumpDelay = 0;
        }

        // UpdateFallFlying() (Elytra)
        var aabb = Player.Entity!.GetBoundingBox();

        // reset fall distance?

        // travelRidden() if player 
        Travel();
    }

    private void Travel()
    {
        // TODO: Swimming && !isPassenger()
        // TODO: Ability to fly

        var gravity = PhysicsConst.Gravity;
        var isFalling = Player.Entity!.Velocity.Y <= 0.0;
        if (isFalling && Player.Entity!.GetEffectLevel(EffectType.SlowFalling).HasValue)
        {
            gravity = PhysicsConst.SlowFallingGravity;
        }

        var dx = movementInput.StrafeImpulse * 0.98f;
        var dz = movementInput.ForwardImpulse * 0.98f;
        var block = World.GetBlockAt((Position)Player.Entity!.Position);

        // LivingEntity.java:2015
        // && !abilities.canFly
        if (State.WasTouchingWater)
        {
            TravelWater(isFalling, dx, dz);
        } // else if in laval

        // else if fall flying (elytra)
        else
        {
            TravelNormally(gravity, dx, dz);
        }
    }

    private void TravelNormally(double gravity, double dx, double dz)
    {
        var blockBelow = World.GetBlockAt(new(
                                              Player.Entity!.Position.X,
                                              Player.Entity!.Position.Y - 0.5f,
                                              Player.Entity!.Position.Z));
        var friction = PhysicsConst.GetBlockFriction(blockBelow.Info.Type);
        var speedFactor = Player.Entity.IsOnGround ? friction * 0.91f : 0.91f;

        var frictionInfluencedSpeed = Player.Entity.IsOnGround
            ? speedAttribute.Value * (0.21600002f / Math.Pow(speedFactor, 3))
            : State.IsSprinting
                ? PhysicsConst.SprintingFlyingSpeed
                : PhysicsConst.FlyingSpeed;

        MoveRelative(dx, 0, dz, (float)frictionInfluencedSpeed, Player.Entity.Yaw);
        CheckClimbable();
        Move();

        var vel = (Vector3)Player.Entity.Velocity.Clone();

        var blockAtFeet = World.GetBlockAt((Position)Player.Entity.Position);
        if ((State.HorizontalCollision || movementInput.Controls.JumpingKeyDown)
            && (WorldUtils.IsOnClimbable(Player, World, ref State.LastClimbablePosition) ||
                blockAtFeet.Info.Type == BlockType.PowderSnow))
        {
            vel = new(vel.X, 0.2, vel.Z);
        }

        var velY = vel.Y;
        var levitation = Player.Entity!.GetEffectLevel(EffectType.Levitation);
        if (levitation.HasValue)
        {
            velY += ((0.05d * (levitation.Value + 1)) - velY) * 0.2d;
        }
        else if (!World.IsChunkLoaded(World.ToChunkCoordinates((Position)Player.Entity.Position)))
        {
            velY = Player.Entity.Position.Y > World.MinY ? 0.1d : 0.0;
        }
        else
        {
            velY -= gravity;
        }

        playerVelocity.Set(vel.X * speedFactor, velY * 0.98f, vel.Z * speedFactor);
    }

    private void CheckClimbable()
    {
        if (!WorldUtils.IsOnClimbable(Player, World, ref State.LastClimbablePosition))
        {
            return;
        }

        var x = Math.Clamp(Player.Entity!.Velocity.X, -PhysicsConst.ClimbingSpeed, PhysicsConst.ClimbingSpeed);
        var z = Math.Clamp(Player.Entity!.Velocity.Z, -PhysicsConst.ClimbingSpeed, PhysicsConst.ClimbingSpeed);
        var y = Math.Max(Player.Entity!.Velocity.Y, -PhysicsConst.ClimbingSpeed);

        var blockAtFeet = World.GetBlockAt((Position)Player.Entity.Position);

        // && !abilities.flying
        if (y < 0.0 && blockAtFeet.Info.Type != BlockType.Scaffolding && movementInput.Controls.SneakingKeyDown)
        {
            y = 0.0;
        }

        playerVelocity.Set(x, y, z);
    }

    private void TravelWater(bool isFalling, double dx, double dz)
    {
        var lastY = Player.Entity!.Position.Y;
        var waterFactor = State.IsSprinting ? 0.9f : 0.8f;
        var speedFactor = 0.02f;
        var depthStrider = 0.0f; // TODO: get Depth strider level
        depthStrider = MathF.Max(3.0f, depthStrider);

        if (!Player.Entity!.IsOnGround)
        {
            depthStrider *= 0.5f;
        }

        if (depthStrider > 0.0)
        {
            waterFactor += (0.54600006F - waterFactor) * depthStrider / 3.0F;
            speedFactor += ((float)speedAttribute.Value - speedFactor) * depthStrider / 3.0F;
        }

        if (Player.Entity!.GetEffectLevel(EffectType.DolphinsGrace).HasValue)
        {
            waterFactor = 0.96f;
        }

        MoveRelative(dx, 0, dz, speedFactor, Player.Entity!.Yaw);
        Move();

        // TODO: Finish water movement
    }

    private void Move()
    {
        var vec = Player.Entity!.Velocity.Clone();

        if (State.StuckSpeedMultiplier.LengthSquared() > 1.0E-7D)
        {
            vec.Multiply(State.StuckSpeedMultiplier);
            State.StuckSpeedMultiplier = Vector3.Zero;

            playerVelocity.Set(0, 0, 0);
        }

        MaybeBackOffFromEdge(vec);
        var vec3 = Collide(vec);

        var length = vec3.LengthSquared();
        if (length > 1.0E-7D)
        {
            // reset fall distance
            playerPosition.Add(vec3);
        }

        var collidedX = Math.Abs(vec.X - vec3.X) > 1.0E-5F;
        var collidedZ = Math.Abs(vec.Z - vec3.Z) > 1.0E-5F;
        State.HorizontalCollision = collidedX || collidedZ;
        State.VerticalCollision = Math.Abs(vec.Y - vec3.Y) > 1.0E-5F;
        State.MinorHorizontalCollision = IsCollisionMinor(vec3);

        Player.Entity!.IsOnGround = State.VerticalCollision && vec.Y < 0;

        if (State.HorizontalCollision)
        {
            if (collidedX)
            {
                playerVelocity.SetX(0.0);
            }
            else
            {
                playerVelocity.SetZ(0.0);
            }
        }
        else
        {
            State.MinorHorizontalCollision = false;
        }

        if (State.VerticalCollision)
        {
            playerVelocity.SetY(0.0);
        }
    }

    private Vector3 Collide(Vector3 vec)
    {
        var aabb = Player.Entity!.GetBoundingBox();
        var collidedVec = vec.LengthSquared() == 0.0 ? vec : CheckBoundingBoxCollisions(vec.X, vec.Y, vec.Z, aabb);

        var collidedX = Math.Abs(vec.X - collidedVec.X) > 1.0E-5;
        var collidedY = Math.Abs(vec.Y - collidedVec.Y) > 1.0E-5;
        var collidedZ = Math.Abs(vec.Z - collidedVec.Z) > 1.0E-5;
        var hitGround = Player.Entity!.IsOnGround || (collidedY && vec.Y < 0.0);

        if (hitGround && (collidedX || collidedZ))
        {
            var collidedUpXz = CheckBoundingBoxCollisions(
                vec.X, PhysicsConst.MaxUpStep, vec.Z, aabb);

            var collidedUp0 = CheckBoundingBoxCollisions(
                0, PhysicsConst.MaxUpStep, 0,
                aabb.Clone().Expand(vec.X, 0, vec.Z));

            if (collidedUp0.Y < PhysicsConst.MaxUpStep)
            {
                var collidedAbove = CheckBoundingBoxCollisions(
                        vec.X, 0.0, vec.Z,
                        aabb.Clone().Offset(collidedUp0.X, collidedUp0.Y, collidedUp0.Z))
                   .Plus(collidedUp0);

                if (collidedAbove.HorizontalLengthSquared() > collidedUpXz.HorizontalLengthSquared())
                {
                    collidedUpXz = collidedAbove;
                }
            }

            if (collidedUpXz.HorizontalLengthSquared() > collidedVec.HorizontalLengthSquared())
            {
                collidedUpXz.Add(
                    CheckBoundingBoxCollisions(
                        0.0, -collidedUpXz.Y + vec.Y, 0.0,
                        aabb.Clone().Offset(collidedUpXz.X, collidedUpXz.Y, collidedUpXz.Z)));

                return collidedUpXz;
            }
        }

        return collidedVec;
    }

    private bool IsCollisionMinor(Vector3 vec)
    {
        var yawRadians = Player.Entity!.Yaw * (Math.PI / 180.0);
        var sin = Math.Sin(yawRadians);
        var cos = Math.Cos(yawRadians);

        var x = (movementInput.StrafeImpulse * cos * 0.98) - (movementInput.ForwardImpulse * sin * 0.98);
        var z = (movementInput.ForwardImpulse * cos * 0.98) - (movementInput.StrafeImpulse * sin * 0.98);

        var s1 = Math.Pow(x, 2) + Math.Pow(z, 2);
        var s2 = Math.Pow(vec.X, 2) + Math.Pow(vec.Z, 2);

        if (s1 < 1.0E-5F || s2 < 1.0E-5F)
        {
            return false;
        }

        var d6 = (x * vec.X) + (z * vec.Z);
        var d7 = Math.Acos(d6 / Math.Sqrt(s1 * s2));
        return d7 < 0.13962634F;
    }

    private MutableVector3 CheckBoundingBoxCollisions(double dx, double dy, double dz, Aabb aabb)
    {
        // TODO: Entity collision boxes
        // TODO: World border collision

        var mutableAabb = aabb.Clone();

        var shapes = WorldUtils.GetWorldBoundingBoxes(
            aabb.Clone().Expand(dx, dy, dz),
            World,
            Data);

        if (shapes.Length == 0)
        {
            return new(dx, dy, dz);
        }

        // check for collisions
        var mX = dx;
        var mY = dy;
        var mZ = dz;

        if (mY != 0.0)
        {
            mY = CalculateAxisOffset(Axis.Y, mutableAabb, shapes, mY);
            if (mY != 0.0)
            {
                mutableAabb.Offset(0, mY, 0);
            }
        }

        var zDominant = Math.Abs(mX) < Math.Abs(mZ);
        if (zDominant && mZ != 0.0)
        {
            mZ = CalculateAxisOffset(Axis.Z, mutableAabb, shapes, mZ);
            if (mZ != 0.0)
            {
                mutableAabb.Offset(0, 0, mZ);
            }
        }

        if (mX != 0.0)
        {
            mX = CalculateAxisOffset(Axis.X, mutableAabb, shapes, mX);
            if (mX != 0.0)
            {
                mutableAabb.Offset(mX, 0, 0);
            }
        }

        if (!zDominant && mZ != 0.0)
        {
            mZ = CalculateAxisOffset(Axis.Z, aabb, shapes, mZ);
        }

        return new(mX, mY, mZ);
    }

    private double CalculateAxisOffset(Axis axis, Aabb aabb, Aabb[] shapes, double displacement)
    {
        var value = displacement;
        if (value == 0.0 || Math.Abs(displacement) < 1.0E-7)
        {
            return 0.0;
        }

        foreach (var shape in shapes)
        {
            value = shape.CalculateMaxOffset(aabb, axis, value);
        }

        return value;
    }

    private void MaybeBackOffFromEdge(MutableVector3 vec)
    {
        // TODO Fix differences with java
        // Player.java:1054
        // && this.IsAboveGround()
        if (vec.Y > 0 || !movementInput.Controls.SneakingKeyDown)
        {
            return;
        }

        var x = vec.X;
        var z = vec.Z;
        var entity = Player.Entity!;

        // TODO: Check for entity collisions, not only block collisions
        const double increment = 0.05d;
        while (x != 0.0D && !WorldUtils.CollidesWithWorld(entity.GetBoundingBox().Offset(x, -1.0, 0.0), World, Data))
        {
            x = x switch
            {
                < increment and >= -increment => 0.0,
                > 0.0 => x - increment,
                _ => x + increment
            };
        }

        while (z != 0.0D && !WorldUtils.CollidesWithWorld(entity.GetBoundingBox().Offset(0.0, -1.0, z), World, Data))
        {
            z = z switch
            {
                < increment and >= -increment => 0.0,
                > 0.0 => z - increment,
                _ => z + increment
            };
        }

        while (x != 0.0D && z != 0.0D &&
            !WorldUtils.CollidesWithWorld(entity.GetBoundingBox().Offset(x, -1.0, z), World, Data))
        {
            x = x switch
            {
                < increment and >= -increment => 0.0,
                > 0.0 => x - increment,
                _ => x + increment
            };

            z = z switch
            {
                < increment and >= -increment => 0.0,
                > 0.0 => z - increment,
                _ => z + increment
            };
        }

        vec.SetX(x);
        vec.SetZ(z);
    }

    private void MoveRelative(double dx, double dy, double dz, float scale, float yaw)
    {
        var length = (dx * dx) + (dy * dy) + (dz * dz);
        if (length < 1.0E-7D)
        {
            return;
        }

        if (length > 1.0)
        {
            // normalize
            dx = 1 / length * dx;
            dy = 1 / length * dy;
            dz = 1 / length * dz;
        }

        dx *= scale;
        dy *= scale;
        dz *= scale;

        var sin = MathF.Sin(yaw * (MathF.PI / 180.0F));
        var cos = MathF.Cos(yaw * (MathF.PI / 180.0F));

        playerVelocity.Add(
            (dx * cos) - (dz * sin),
            dy,
            (dz * cos) + (dx * sin));
    }

    private void TryJumping()
    {
        var fluidHeight = State.LavaHeight > 0.0
            ? State.LavaHeight
            : State.WaterHeight;

        var isInWater = State.WasTouchingWater && fluidHeight > 0.0;
        if (!isInWater || (Player.Entity!.IsOnGround && fluidHeight <= PhysicsConst.FluidJumpThreshold))
        {
            if (State.LavaHeight <= 0.0 ||
                (Player.Entity!.IsOnGround && fluidHeight <= PhysicsConst.FluidJumpThreshold))
            {
                if ((Player.Entity!.IsOnGround || (isInWater && fluidHeight <= PhysicsConst.FluidJumpThreshold)) &&
                    State.NoJumpDelay == 0)
                {
                    JumpFromGround();
                    State.NoJumpDelay = PhysicsConst.JumpDelay;
                }
            }
            else
            {
                JumpInFluid();
            }
        }
        else
        {
            JumpInFluid();
        }
    }

    private void JumpFromGround()
    {
        var jumpBoostFactor = 0.1d * (Player.Entity!.GetEffectLevel(EffectType.JumpBoost) + 1) ?? 0;

        playerVelocity.SetY(
            (0.42 * WorldUtils.GetBlockJumpFactor((Position)Player.Entity.Position, World)) + jumpBoostFactor);

        if (!State.IsSprinting)
        {
            return;
        }

        var yaw = Player.Entity!.Yaw * (MathF.PI / 180.0f);

        playerVelocity.Add(-0.2 * Math.Sin(yaw), 0, 0.2 * Math.Cos(yaw));
    }

    private void JumpInFluid()
    {
        playerVelocity.Add(0.0, PhysicsConst.FluidJumpFactor, 0.0);
    }

    private void UpdateSprinting()
    {
        var hasSprintingImpulse = movementInput.HasSprintingImpulse(State.WasUnderwater);

        // TODO: fix differences with java
        // LocalPlayer.java:975 canStartSprinting()
        // hasEnoughFoodToStartSprinting() && !isUsingItem()
        // + effects + vehicle
        var canStartSprinting = !State.IsSprinting && hasSprintingImpulse;

        // var onGround = this.Player.Entity!.IsOnGround; // isPassenger() -> getVehicle.IsOnGround
        // var flag = !this.input.Controls.SneakingKeyDown && !hasSprintingImpulse;
        //
        // if ((onGround || this.state.WasUnderwater) && flag && canStartSprinting)
        // {
        //     
        // }

        if ((!State.WasTouchingWater || State.WasUnderwater) && canStartSprinting &&
            movementInput.Controls.SprintingKeyDown)
        {
            // TODO: Prob add sprinting attribute
            speedAttribute.AddModifier(sprintingModifier);
            State.IsSprinting = true;
            OnSprintingChanged.Dispatch(this, State.IsSprinting);
        }

        if (State.IsSprinting)
        {
            var cannotSprint = !movementInput.HasForwardImpulse(); // || hasEnoughFoodToStartSprinting()
            var stopSprinting = cannotSprint
                || (State.HorizontalCollision && !State.MinorHorizontalCollision)
                || (State.WasTouchingWater && !State.WasUnderwater);

            // isSwimming() LocalPlayer.java:676
            // else
            if (stopSprinting)
            {
                speedAttribute.RemoveModifier(sprintingModifier.Uuid);
                State.IsSprinting = false;
                OnSprintingChanged.Dispatch(this, State.IsSprinting);
            }
        }
    }

    private void PushTowardsClosestSpace(double x, double z)
    {
        var pos = new Position(x, Player.Entity!.Position.Y, z);

        if (!CollidesWithSuffocatingBlock(pos))
        {
            return;
        }

        var nX = x - pos.X;
        var nZ = z - pos.Z;
        var diff = double.MaxValue;
        Vector3? direction = null;

        foreach (var axis in XzAxisVectors)
        {
            var coord = axis.ChooseValueForAxis(nX, 0.0, nZ);
            coord = axis.IsPositiveAxisVector()
                ? 1.0 - coord
                : coord;

            if (coord >= diff || CollidesWithSuffocatingBlock((Position)axis.Plus(pos)))
            {
                continue;
            }

            direction = axis;
            diff = coord;
        }

        if (direction is null)
        {
            return;
        }

        if (direction.X != 0.0)
        {
            playerVelocity.SetX(0.1d * direction.X);
        }
        else
        {
            playerVelocity.SetZ(0.1 * direction.Z);
        }
    }

    private bool CollidesWithSuffocatingBlock(Position position)
    {
        var bb = Player.Entity!.GetBoundingBox();
        bb.Min.Set(position.X, 0, position.Z);
        bb.Max.Set(position.X + 1.0, 0, position.Z + 1.0);
        bb.Deflate(1.0E-7D, 1.0E-7D, 1.0E-7D);

        return WorldUtils.CollidesWithWorld(bb, World, Data);
    }

    private void TruncateVelocity(MutableVector3 vec)
    {
        if (Math.Abs(vec.X) < PhysicsConst.VelocityThreshold)
        {
            vec.SetX(0);
        }

        if (Math.Abs(vec.Y) < PhysicsConst.VelocityThreshold)
        {
            vec.SetY(0);
        }

        if (Math.Abs(vec.Z) < PhysicsConst.VelocityThreshold)
        {
            vec.SetZ(0);
        }
    }
}
