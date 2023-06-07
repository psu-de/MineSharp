using MineSharp.Core.Common;
using MineSharp.Core.Common.Entities.Attributes;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Entities;
using MineSharp.Data;
using MineSharp.World;
using NLog;
using Attribute = System.Attribute;

/*
This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at http://mozilla.org/MPL/2.0/.

Thanks to https://github.com/ConcreteMC/Alex
Especially these files:
- https://github.com/ConcreteMC/Alex/blob/master/src/Alex/Entities/Components/PhysicsComponent.cs
- https://github.com/ConcreteMC/Alex/blob/master/src/Alex/Entities/Components/MovementComponent.cs
*/

namespace MineSharp.Physics;

public class PhysicsEngine
{
    private static ILogger Logger = LogManager.GetCurrentClassLogger();

    public readonly Entity Player;
    public readonly PlayerState PlayerState;
    public readonly IWorld World;
    
    private readonly MinecraftData _data;
    private readonly PhysicsConst _physicsConst;

    private readonly int _honeyBlock;
    private readonly int _ladderBlock;
    private readonly int _vineBlock;
    private readonly int _waterBlock;
    private readonly int _jumpBoostEffect;

    public PhysicsEngine(MinecraftData data, Entity player, IWorld world)
    {
        this._data = data;
        this.World = world;
        this.Player = player;
        this.PlayerState = new PlayerState(data, player);

        this._physicsConst = new PhysicsConst(data);

        this._honeyBlock = this._data.Blocks.GetByName("honey_block").Id;
        this._ladderBlock = this._data.Blocks.GetByName("ladder").Id;
        this._vineBlock = this._data.Blocks.GetByName("vine").Id;
        this._waterBlock = this._data.Blocks.GetByName("water").Id;
        this._jumpBoostEffect = this._data.Effects.GetByName("JumpBoost").Id;
    }

    public void SimulatePlayer(MovementControls controls)
    {
        var playerBB = PhysicsConst.GetPlayerBoundingBox(this.Player.Position);

        var waterBB = playerBB.Clone().Contract(0.001d, 0.4d, 0.001d);
        var lavaBB = playerBB.Clone().Contract(0.1d, 0.4d, 0.1d);

        this.PlayerState.IsInWater = this.CheckInWaterAndAppyCurrent(waterBB, this.Player.Velocity);

        var onGround = this.Player.IsOnGround;
        if (!this.Player.Attributes.TryGetValue(
                PhysicsConst.MovementSpeedAttribute, out var movementFactorAttr))
        {
            movementFactorAttr = new MineSharp.Core.Common.Entities.Attributes.Attribute(
                PhysicsConst.MovementSpeedAttribute,
                PhysicsConst.PlayerSpeed,
                Array.Empty<Modifier>());
        }

        bool hasSprintAttr = movementFactorAttr.Modifiers.TryGetValue(PhysicsConst.SprintingUUID, out var spMod);
        
        if (controls.Sprint)
        {
            if (!hasSprintAttr)
            {
                spMod = new Modifier(PhysicsConst.SprintingUUID, PhysicsConst.SprintSpeed, ModifierOp.Multiply);
                movementFactorAttr.Modifiers.Add(PhysicsConst.SprintingUUID, spMod);
            }
        } else
        {
            if (hasSprintAttr)
            {
                movementFactorAttr.Modifiers.Remove(PhysicsConst.SprintingUUID);
            }
        }

        var movementFactor = movementFactorAttr.Value;
        var slipperiness = 0.91d;

        if (this.PlayerState.IsInWater)
        {
            movementFactor = 0.2d;
            slipperiness = 0.8d;
        } else
        {
            if (onGround)
            {
                var blockUnder = playerBB.MinY % 1 < 0.05f 
                        ? this.World.GetBlockAt(this.Player.Position.Minus(Vector3.Down)) 
                        : this.World.GetBlockAt(this.Player.Position);
                slipperiness *= this._physicsConst.GetBlockSlipperiness(blockUnder.Info.Id);

                var acceleration = 0.1627714d / Math.Pow(slipperiness, 3);
                movementFactor *= acceleration;
            } else
            {
                movementFactor = 0.02d;
            }
        }


        if (controls.Jump)
        {
            if (onGround)
            {
                var blockUnder = this.World.GetBlockAt(this.Player.Position.Minus(Vector3.Down));
                this.Player.Velocity.Y += 0.42d * (blockUnder.Info.Id == this._honeyBlock ? PhysicsConst.HoneyblockJumpSpeed : 1);

                var effectLevel = this.Player.GetEffectLevel(this._jumpBoostEffect);
                if (effectLevel.HasValue)
                {
                    this.Player.Velocity.Y += 0.1d * effectLevel.Value;
                }
            }
        }

        var heading = this.CalculateHeading(controls);
        // TODO: When swimming rotate heading vector
        //       by Pitch radians 

        var strafing = heading.Z * 0.98d;
        var forward = heading.X * 0.98d;

        if (controls.Sneak)
        {
            strafing *= 0.3d;
            forward *= 0.3d;
        }

        heading = this.ApplyHeading(strafing, forward, movementFactor);

        this.Player.Velocity.Add(heading);

        if (this.IsOnLadder(this.Player.Position))
        {
            this.Player.Velocity.X = Math.Clamp(this.Player.Velocity.X, -PhysicsConst.LadderMaxSpeed, PhysicsConst.LadderMaxSpeed);
            this.Player.Velocity.Z = Math.Clamp(this.Player.Velocity.Z, -PhysicsConst.LadderMaxSpeed, PhysicsConst.LadderMaxSpeed);
            this.Player.Velocity.Y = Math.Max(this.Player.Velocity.Y, controls.Sneak ? 0 : -PhysicsConst.LadderMaxSpeed);
        }

        this.Move(controls, this.TruncateVector(this.Player.Velocity));

        if (this.IsOnLadder(this.Player.Position) && (this.PlayerState.IsCollidedHorizontally || controls.Jump))
        {
            this.Player.Velocity.Y = PhysicsConst.LadderClimbSpeed;
        }

        if (!this.Player.IsOnGround)
        {
            var gravity = PhysicsConst.Gravity;

            if (this.PlayerState.IsInWater)
            {
                gravity /= 4.0d;
            }
            this.Player.Velocity.Subtract(new Vector3(0f, gravity, 0f));
        }

        if (this.PlayerState.IsInWater)
        {
            this.Player.Velocity.Multiply(new Vector3(slipperiness, slipperiness, slipperiness));
        } else
        {
            this.Player.Velocity.Multiply(new Vector3(slipperiness, 0.98d, slipperiness));
        }

        this.Player.Velocity = this.TruncateVector(this.Player.Velocity);
    }

    private Vector3 TruncateVector(Vector3 vector)
    {
        var clone = vector.Clone();
        if (Math.Abs(clone.X) < PhysicsConst.NegligeableVelocity)
            clone.X = 0;
        if (Math.Abs(clone.Y) < PhysicsConst.NegligeableVelocity)
            clone.Y = 0;
        if (Math.Abs(clone.Z) < PhysicsConst.NegligeableVelocity)
            clone.Z = 0;
        return clone;
    }

    private Vector3 CalculateHeading(MovementControls controls)
    {
        var moveVector = Vector3.Zero;

        if (controls.Forward)
            moveVector.X += 1;

        if (controls.Back)
            moveVector.X -= 1;

        if (controls.Left)
            moveVector.Z += 1;

        if (controls.Right)
            moveVector.Z -= 1;

        return moveVector;
    }

    private void Move(MovementControls controls, Vector3 amount)
    {
        var wasOnGround = this.Player.IsOnGround;

        var collideY = this.CheckY(ref amount, false);
        var collideX = this.CheckX(ref amount, false);
        var collideZ = this.CheckZ(ref amount, false);

        if (!collideX && this.CheckX(ref amount, true))
        {
            collideX = true;
        }

        if (collideZ && this.CheckZ(ref amount, true))
        {
            collideZ = true;
        }

        if (controls.Sneak && wasOnGround)
        {
            this.FixSneaking(ref amount);
        }

        this.PlayerState.IsCollidedHorizontally = collideX || collideZ;
        this.PlayerState.IsCollidedVertically = collideY;

        this.Player.Position.Add(amount);
        this.Player.IsOnGround = this.DetectOnGround();
    }

    private bool DetectOnGround()
    {
        var entityBoundingBox = PhysicsConst.GetPlayerBoundingBox(this.Player.Position).Offset(0, -PhysicsConst.Gravity, 0);

        var offset = 0f;

        if (entityBoundingBox.MinY % 1 < 0.05f)
        {
            offset = -1f;
        }

        var minX = entityBoundingBox.MinX;

        var minZ = entityBoundingBox.MinZ;

        var maxX = entityBoundingBox.MaxX;
        var maxZ = entityBoundingBox.MaxZ;

        var y = (int)Math.Floor(entityBoundingBox.MinY + offset);

        for (var x = (int)Math.Floor(minX); x <= (int)Math.Ceiling(maxX); x++)
        {
            for (var z = (int)Math.Floor(minZ); z <= (int)Math.Ceiling(maxZ); z++)
            {
                var block = this.World.GetBlockAt(new Position(x, y, z));

                if (!this._data.Blocks.IsSolid(block.Info.Id))
                    continue;

                var coords = new Vector3(x, y, z);
                
                foreach (var box in this._data.Collisions.GetForBlock(block).Select(x => new AABB(x).Offset(block.Position!.X, block.Position.Y, block.Position.Z)))
                {
                    if (box.Intersects(entityBoundingBox))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private void FixSneaking(ref Vector3 amount)
    {
        var dX = amount.X;
        var dZ = amount.Z;
        var correctedX = amount.X;
        var correctedZ = amount.Z;
        var increment = 0.05d;

        var boundingBox = PhysicsConst.GetPlayerBoundingBox(this.Player.Position);

        while (dX != 0.0f && !this.GetIntersecting(boundingBox.Offset(dX, -PhysicsConst.MaxFallDistance, 0d)).Any())
        {
            if (dX < increment && dX >= -increment)
                dX = 0.0f;
            else if (dX > 0.0D)
                dX -= increment;
            else
                dX += increment;

            correctedX = dX;
        }

        while (dZ != 0.0f && !this.GetIntersecting(boundingBox.Offset(0d, -PhysicsConst.MaxFallDistance, dZ)).Any())
        {
            if (dZ < increment && dZ >= -increment)
                dZ = 0.0f;
            else if (dZ > 0.0f)
                dZ -= increment;
            else
                dZ += increment;

            correctedZ = dZ;
        }

        while (dX != 0.0f && dZ != 0.0f && !this.GetIntersecting(boundingBox.Offset(dX, -PhysicsConst.MaxFallDistance, dZ)).Any())
        {
            if (dX < increment && dX >= -increment)
                dX = 0.0f;
            else if (dX > 0.0f)
                dX -= increment;
            else
                dX += increment;

            correctedX = dX;


            if (dZ < increment && dZ >= -increment)
                dZ = 0.0f;
            else if (dZ > 0.0f)
                dZ -= increment;
            else
                dZ += increment;

            correctedZ = dZ;
        }

        amount.X = correctedX;
        amount.Z = correctedZ;
    }

    private IEnumerable<AABB> GetIntersecting(AABB box)
    {
        var minX = (int)Math.Floor(box.MinX);
        var maxX = (int)Math.Ceiling(box.MaxX);

        var minZ = (int)Math.Floor(box.MinZ);
        var maxZ = (int)Math.Ceiling(box.MaxZ);

        var minY = (int)Math.Floor(box.MinY);
        var maxY = (int)Math.Ceiling(box.MaxY);

        for (var x = minX; x < maxX; x++)
        {
            for (var y = minY; y < maxY; y++)
            {
                for (var z = minZ; z < maxZ; z++)
                {
                    var coords = new Vector3(x, y, z);
                    var block = this.World.GetBlockAt(coords);

                    if (!this._data.Blocks.IsSolid(block.Info.Id))
                        continue;

                    foreach (var blockBox in this._data.Collisions.GetForBlock(block).Select(x => new AABB(x).Offset(block.Position!.X, block.Position.Y, block.Position.Z)))
                    {
                        if (box.Intersects(blockBox))
                        {
                            yield return blockBox;
                        }
                    }
                }
            }
        }
    }

    private float CollidedWithWorld(Vector3 direction, Vector3 position, double impactVelocity)
    {
        if (direction == Vector3.Down)
        {
            this.Player.IsOnGround = true;
        }
        return 0;
    }

    private bool CheckY(ref Vector3 amount, bool checkOther)
    {
        var beforeAdjustment = amount.Y;

        if (!this.TestTerrainCollisionY(ref amount, out var yCollisionPoint, out var collisionY))
            return false;

        var yVelocity = this.CollidedWithWorld(beforeAdjustment < 0 ? Vector3.Down : Vector3.Up, yCollisionPoint, beforeAdjustment);

        if (MathF.Abs(yVelocity) < 0.005f)
            yVelocity = 0;

        amount.Y = collisionY;

        this.Player.Velocity = new Vector3(this.Player.Velocity.X, yVelocity, this.Player.Velocity.Z);

        return true;
    }

    private bool CheckX(ref Vector3 amount, bool checkOther)
    {
        if (!this.TestTerrainCollisionX(amount, out _, out var collisionX, checkOther))
            return false;
        var climbingResult = this.CheckClimbing(amount, out var yValue);
        if (climbingResult == CollisionResult.ClimbHalfBlock)
        {
            amount.Y = yValue;
        } else
        {
            if (collisionX < 0f)
                collisionX -= 0.005f;

            amount.X += collisionX;
            this.Player.Velocity = new Vector3(0, this.Player.Velocity.Y, this.Player.Velocity.Z);
        }

        return true;
    }

    private bool CheckZ(ref Vector3 amount, bool checkOther)
    {
        if (!this.TestTerrainCollisionZ(amount, out _, out var collisionZ, checkOther))
            return false;

        var climbingResult = this.CheckClimbing(amount, out var yValue);

        if (climbingResult == CollisionResult.ClimbHalfBlock)
        {
            amount.Y = yValue;
        } else
        {
            if (collisionZ < 0f)
                collisionZ -= 0.005f;

            amount.Z += collisionZ;
            this.Player.Velocity = new Vector3(this.Player.Velocity.X, this.Player.Velocity.Y, 0f);
        }

        return true;
    }

    private bool TestTerrainCollisionY(ref Vector3 velocity, out Vector3 collisionPoint, out double result)
    {
        collisionPoint = Vector3.Zero;
        result = velocity.Y;

        bool negative;

        var entityBox = PhysicsConst.GetPlayerBoundingBox(this.Player.Position);
        AABB testBox;

        if (velocity.Y < 0)
        {
            testBox = entityBox.Clone();
            testBox.MinY += velocity.Y;

            negative = true;
        } else
        {
            testBox = entityBox.Clone();
            testBox.MaxY += velocity.Y;

            negative = false;
        }

        double? collisionExtent = null;

        for (var x = (int)Math.Floor(testBox.MinX); x <= (int)Math.Ceiling(testBox.MaxX); x++)
        {
            for (var z = (int)Math.Floor(testBox.MinZ); z <= (int)Math.Ceiling(testBox.MaxZ); z++)
            {
                for (var y = (int)Math.Floor(testBox.MinY); y <= (int)Math.Ceiling(testBox.MaxY); y++)
                {
                    var blockState = this.World.GetBlockAt(new Position(x, y, z));

                    if (!this._data.Blocks.IsSolid(blockState.Info.Id))
                        continue;

                    var coords = new Vector3(x, y, z);

                    foreach (var box in this._data.Collisions.GetForBlock(blockState).Select(c => new AABB(c).Offset(x, y, z)))
                    {
                        if (negative)
                        {
                            if (entityBox.MinY - box.MaxY < 0)
                                continue;
                        } else
                        {
                            if (box.MinY - entityBox.MaxY < 0)
                                continue;
                        }

                        if (testBox.Intersects(box))
                        {
                            if (negative)
                            {
                                if (collisionExtent == null || collisionExtent.Value < box.MaxY)
                                {
                                    collisionExtent = box.MaxY;
                                    collisionPoint = coords;
                                }
                            } else
                            {
                                if (collisionExtent == null || collisionExtent.Value > box.MinY)
                                {
                                    collisionExtent = box.MinY;
                                    collisionPoint = coords;
                                }
                            }
                        }
                    }
                }
            }
        }

        if (collisionExtent != null)
        {
            var extent = collisionExtent.Value;

            double diff;

            if (negative)
                diff = extent - entityBox.MinY;
            else
                diff = extent - entityBox.MaxY;

            result = (float)diff;

            return true;
        }
        return false;
    }

    private bool TestTerrainCollisionX(Vector3 velocity, out Vector3 collisionPoint, out double result, bool includeOther)
    {
        result = velocity.X;
        collisionPoint = Vector3.Zero;

        bool negative;

        var p = this.Player.Position;
        if (includeOther) p = p.Plus(velocity);
        var entityBox = PhysicsConst.GetPlayerBoundingBox(p);
        AABB testBox;

        var min = new Vector3(entityBox.MinX, entityBox.MinY, entityBox.MinZ);
        var max = new Vector3(entityBox.MaxX, entityBox.MaxY, entityBox.MaxZ);

        if (velocity.X < 0)
        {
            min.X += velocity.X;
            negative = true;
        } else
        {
            max.X += velocity.X;
            negative = false;
        }

        if (includeOther)
        {
            if (velocity.Z < 0)
            {
                min.Z += velocity.Z;
            } else
            {
                max.Z += velocity.Z;
            }

            if (velocity.Y < 0)
            {
                min.Y += velocity.Y;
            } else
            {
                max.Y += velocity.Y;
            }
        }

        testBox = new AABB(min.X, min.Y, min.Z, max.X, max.Y, max.Z);

        var minX = testBox.MinX;
        var minZ = testBox.MinZ;

        var maxX = testBox.MaxX;
        var maxZ = testBox.MaxZ;

        var minY = testBox.MinY;
        var maxY = testBox.MaxY;

        double? collisionExtent = null;

        for (var x = (int)Math.Floor(minX); x <= (int)Math.Ceiling(maxX); x++)
        {
            for (var z = (int)Math.Floor(minZ); z <= (int)Math.Ceiling(maxZ); z++)
            {
                for (var y = (int)Math.Floor(minY); y <= (int)Math.Ceiling(maxY); y++)
                {
                    var blockState = this.World.GetBlockAt(new Position(x, y, z));

                    if (!this._data.Blocks.IsSolid(blockState.Info.Id))
                        continue;

                    var coords = new Vector3(x, y, z);

                    foreach (var box in this._data.Collisions.GetForBlock(blockState).Select(x => new AABB(x).Offset(blockState.Position!.X, blockState.Position.Y, blockState.Position.Z)))
                    {
                        if (box.MaxY <= minY) continue;

                        if (negative)
                        {
                            if (box.MaxX <= minX)
                                continue;

                            if (entityBox.MinX - box.MaxX < 0)
                                continue;
                        } else
                        {
                            if (box.MinX >= maxX)
                                continue;

                            if (box.MinX - entityBox.MaxX < 0)
                                continue;
                        }

                        if (testBox.Intersects(box))
                        {
                            if (negative)
                            {
                                if (collisionExtent != null && !(collisionExtent.Value < box.MaxX))
                                    continue;

                                collisionExtent = box.MaxX;
                                collisionPoint = coords;
                            } else
                            {
                                if (collisionExtent != null && !(collisionExtent.Value > box.MinX))
                                    continue;

                                collisionExtent = box.MinX;
                                collisionPoint = coords;
                            }
                        }
                    }
                }
            }
        }

        if (collisionExtent != null)
        {
            double diff;

            if (negative)
            {
                diff = collisionExtent.Value - minX + 0.01f;
            } else
            {
                diff = collisionExtent.Value - maxX;
            }

            result = (float)diff;
            return true;
        }

        return false;
    }

    private bool TestTerrainCollisionZ(Vector3 velocity, out Vector3 collisionPoint, out double result, bool includeOther)
    {
        result = velocity.Z;
        collisionPoint = Vector3.Zero;

        bool negative;

        var p = this.Player.Position;
        if (includeOther) p = p.Plus(velocity);
        var entityBox = PhysicsConst.GetPlayerBoundingBox(p);
        AABB testBox;

        var min = new Vector3(entityBox.MinX, entityBox.MinY, entityBox.MinZ);
        var max = new Vector3(entityBox.MaxX, entityBox.MaxY, entityBox.MaxZ);

        if (velocity.Z < 0)
        {
            min.Z += velocity.Z;
            negative = true;
        } else
        {
            max.Z += velocity.Z;
            negative = false;
        }

        if (includeOther)
        {
            if (velocity.X < 0)
            {
                min.X += velocity.X;
            } else
            {
                max.X += velocity.X;
            }

            if (velocity.Y < 0)
            {
                min.Y += velocity.Y;
            } else
            {
                max.Y += velocity.Y;
            }
        }

        testBox = new AABB(min.X, min.Y, min.Z, max.X, max.Y, max.Z);

        var minX = testBox.MinX;
        var minZ = testBox.MinZ;

        var maxX = testBox.MaxX;
        var maxZ = testBox.MaxZ;

        var minY = testBox.MinY;
        var maxY = testBox.MaxY;

        double? collisionExtent = null;

        for (var x = (int)Math.Floor(minX); x <= (int)Math.Ceiling(maxX); x++)
        {
            for (var z = (int)Math.Floor(minZ); z <= (int)Math.Ceiling(maxZ); z++)
            {
                for (var y = (int)Math.Floor(minY); y <= (int)Math.Ceiling(maxY); y++)
                {
                    var blockState = this.World.GetBlockAt(new Position(x, y, z));

                    if (!this._data.Blocks.IsSolid(blockState.Info.Id))
                        continue;

                    var coords = new Vector3(x, y, z);

                    foreach (var box in this._data.Collisions.GetForBlock(blockState).Select(x => new AABB(x).Offset(blockState.Position!.X, blockState.Position.Y, blockState.Position.Z)))
                    {
                        if (box.MaxY <= minY) continue;

                        if (negative)
                        {
                            if (box.MaxZ <= minZ)
                                continue;

                            if (entityBox.MinZ - box.MaxZ < 0)
                                continue;
                        } else
                        {
                            if (box.MinZ >= maxZ)
                                continue;

                            if (box.MinZ - entityBox.MaxZ < 0)
                                continue;
                        }

                        if (testBox.Intersects(box))
                        {
                            if (negative)
                            {
                                if (collisionExtent == null || collisionExtent.Value < box.MaxZ)
                                {
                                    collisionExtent = box.MaxZ;
                                    collisionPoint = coords;
                                }
                            } else
                            {
                                if (collisionExtent == null || collisionExtent.Value > box.MinZ)
                                {
                                    collisionExtent = box.MinZ;
                                    collisionPoint = coords;
                                }
                            }
                        }
                    }
                }
            }
        }

        if (collisionExtent != null) // Collision detected, adjust accordingly
        {
            var cp = collisionExtent.Value;

            double diff;

            if (negative)
                diff = cp - minZ + 0.01f;
            else
                diff = cp - maxZ;

            //velocity.Z = (float)diff;	
            result = (float)diff;

            //	if (Entity is Player p)
            //		Log.Debug($"ColZ, Distance={diff}, MinZ={(minZ)} MaxZ={maxZ} PointOfCollision={cp} (negative: {negative})");

            return true;
        }

        return false;
    }


    private CollisionResult CheckClimbing(Vector3 amount, out double yValue)
    {
        yValue = amount.Y;
        var canJump = false;
        var canClimb = false;

        if (this.Player.IsOnGround && Math.Abs(this.Player.Velocity.Y) < 0.001f)
        {
            canJump = true;
            var adjusted = PhysicsConst.GetPlayerBoundingBox(this.Player.Position.Plus(amount));
            var intersecting = this.GetIntersecting(adjusted);

            var targetY = 0d;

            foreach (var box in intersecting)
            {
                var yDifference = box.MaxY - adjusted.MinY;
                if (yDifference > PhysicsConst.StepHeight)
                {
                    canJump = false;
                    break;
                }

                if (yDifference > targetY)
                    targetY = yDifference;
            }

            if (canJump && targetY > 0f)
            {
                adjusted = PhysicsConst.GetPlayerBoundingBox(this.Player.Position.Plus(new Vector3(amount.X, targetY, amount.Z)));

                if (this.GetIntersecting(adjusted).Any(
                        bb => bb.MaxY > adjusted.MinY && bb.MinY <= adjusted.MaxY))
                {
                    canJump = false;
                }
            } else
            {
                canJump = false;
            }

            if (canJump)
            {
                yValue = targetY;
            }
        }

        if (canClimb)
            return CollisionResult.VerticalClimb;

        if (canJump)
            return CollisionResult.ClimbHalfBlock;

        return CollisionResult.DoNothing;
    }

    private Vector3 ApplyHeading(double strafe, double forward, double multiplier)
    {
        var speed = Math.Sqrt(strafe * strafe + forward * forward);

        if (speed < 0.01f)
            return Vector3.Zero;

        speed = multiplier / Math.Max(speed, 1f);

        strafe *= speed;
        forward *= speed;

        var rotationYaw = this.Player.Yaw;
        var sinYaw = MathF.Sin(rotationYaw * MathF.PI / 180.0F);
        var cosYaw = MathF.Cos(rotationYaw * MathF.PI / 180.0F);

        return new Vector3(strafe * cosYaw - forward * sinYaw, 0, forward * cosYaw + strafe * sinYaw);
    }

    private float GetRenderedDepth(Block block)
    {
        if (this._physicsConst.WaterLikeBlocks.Contains(block.Info.Id)) return 0;
        if (block.GetProperty<bool>("waterlogged")) return 0;
        if (block.Info.Id != this._waterBlock) return -1;
        var meta = block.Metadata;
        return meta >= 8 ? 0 : meta;
    }

    private bool CheckInWaterAndAppyCurrent(AABB bb, Vector3 vel)
    {
        var acc = new Vector3(0, 0, 0);
        var waterBlocks = new List<Block>();
        var cursor = new Vector3(0, 0, 0);

        for (cursor.Y = Math.Floor(bb.MinY); cursor.Y <= Math.Floor(bb.MaxY); cursor.Y++)
        {
            for (cursor.Z = Math.Floor(bb.MinZ); cursor.Z <= Math.Floor(bb.MaxZ); cursor.Z++)
            {
                for (cursor.X = Math.Floor(bb.MinX); cursor.X <= Math.Floor(bb.MaxX); cursor.X++)
                {
                    var block = this.World.GetBlockAt(cursor);
                    if (block.Info.Id == this._waterBlock || this._physicsConst.WaterLikeBlocks.Contains(block.Info.Id) || block.GetProperty<bool>("waterlogged"))
                    {
                        var waterLevel = cursor.Y + 1 - (this.GetRenderedDepth(block) + 1) / 9;
                        if (Math.Ceiling(bb.MaxY) >= waterLevel) 
                            waterBlocks.Add(block);
                    }
                }
            }
        }

        var isInWater = waterBlocks.Count > 0;
        foreach (var block in waterBlocks)
        {
            var curLevel = this.GetRenderedDepth(block);
            var flow = new Vector3(0, 0, 0);
            var offsets = new[] {
                new Vector3(0, 0, 1),
                new Vector3(-1, 0, 0),
                new Vector3(0, 0, -1),
                new Vector3(1, 0, 0)
            };
            var p = (Vector3)block.Position!;

            foreach (var offset in offsets)
            {
                var adjBlock = this.World.GetBlockAt(p.Plus(offset));
                var adjLevel = this.GetRenderedDepth(adjBlock);
                if (adjLevel < 0)
                {
                    if (adjBlock.Info.BoundingBox != "empty")
                    {
                        var adjLevel2 = this.GetRenderedDepth(this.World.GetBlockAt(p.Plus(offset).Plus(Vector3.Down)));
                        if (adjLevel2 >= 0)
                        {
                            var f = adjLevel2 - (curLevel - 8);
                            flow.X += offset.X * f;
                            flow.Z += offset.Z * f;
                        }
                    }
                } else
                {
                    var f = adjLevel - curLevel;
                    flow.X += offset.X * f;
                    flow.Z += offset.Z * f;
                }
            }

            if (block.Metadata >= 8)
            {
                foreach (var offset in offsets)
                {
                    var adjBlock = this.World.GetBlockAt(p.Plus(offset));
                    var adjUpBlock = this.World.GetBlockAt(p.Plus(offset).Plus(Vector3.Up));
                    if (adjBlock.Info.BoundingBox != "empty" || adjUpBlock.Info.BoundingBox != "empty")
                    {
                        flow = flow.Normalized().Plus(new Vector3(0, -6, 0));
                    }
                }
            }

            flow = flow.Normalized();
            acc.Add(flow);
        }

        var len = acc.Length();

        if (len > 0)
        {
            vel.X += acc.X / len * 0.014;
            vel.Y += acc.Y / len * 0.014;
            vel.Z += acc.Z / len * 0.014;
        }

        return isInWater;
    }

    private bool IsOnLadder(Vector3 pos)
    {
        var block = this.World.GetBlockAt(pos);
        return block.Info.Id == this._ladderBlock || block.Info.Id == this._vineBlock;
    }

    internal enum CollisionResult
    {
        DoNothing,
        ClimbHalfBlock,
        VerticalClimb
    }
}