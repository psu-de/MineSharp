using MineSharp.Core.Logging;
using MineSharp.Core.Types;
using MineSharp.Data.Blocks;
using MineSharp.Data.Effects;
using MineSharp.Data.Entities;

/*
 Thanks to https://github.com/ConcreteMC/Alex
 Especially these files:
  - https://github.com/ConcreteMC/Alex/blob/master/src/Alex/Entities/Components/PhysicsComponent.cs
  - https://github.com/ConcreteMC/Alex/blob/master/src/Alex/Entities/Components/MovementComponent.cs
*/

namespace MineSharp.Physics
{
    public class PhysicsEngine
    {
        static Logger Logger = Logger.GetLogger();

        public Player Player;
        public PlayerState PlayerState;
        public World.World World;

        public PhysicsEngine(Player player, World.World world)
        {
            this.World = world;
            this.Player = player;
            this.PlayerState = new PlayerState(player);
        }

        public void SimulatePlayer(MovementControls controls)
        {
            Logger.Debug2($"---- ---- ---- Physics Tick ---- ---- ----");
            Logger.Debug2($"Start: Vel={this.Player.Velocity} Pos={this.Player.Position}");
            Logger.Debug2($"       OnGround={this.Player.IsOnGround}");

            var playerBB = GetPlayerBB(this.Player.Position);
            var onGround = this.Player.IsOnGround;
            if (!this.Player.Attributes.TryGetValue(
                PhysicsConst.MovementSpeedAttribute, out var movementFactorAttr))
            {
                movementFactorAttr = new Core.Types.Attribute(
                    PhysicsConst.MovementSpeedAttribute,
                    PhysicsConst.PlayerSpeed,
                    new List<Modifier>());
            }
            
            if (controls.Sprint)
            {
                if(!movementFactorAttr.Modifiers.TryGetValue(PhysicsConst.SprintingUUID, out var spMod)) {
                    spMod = new Modifier(PhysicsConst.SprintingUUID, PhysicsConst.SprintSpeed, ModifierOp.Multiply);
                    movementFactorAttr.Modifiers.Add(PhysicsConst.SprintingUUID, spMod);
                }
            }

            var movementFactor = movementFactorAttr.Value;
            Logger.Debug2($"MovementFactor: {movementFactor}, sprinting={controls.Sprint}");
            var slipperiness = 0.91d;

            if (this.PlayerState.IsInWater)
            {
                movementFactor = 0.2d;
                slipperiness = 0.8d;
            }
            else
            {
                if (onGround)
                {
                    var blockUnder =
                        playerBB.MinY % 1 < 0.05f ?
                        this.World.GetBlockAt(this.Player.Position.Minus(Vector3.Down)) :
                        this.World.GetBlockAt(this.Player.Position);
                    slipperiness *= PhysicsConst.GetBlockSlipperiness(blockUnder.Id);

                    var acceleration = 0.1627714d / Math.Pow(slipperiness, 3);
                    movementFactor *= acceleration;
                }
                else
                {
                    movementFactor = 0.02d;
                }
            }

            Logger.Debug2($"MovementFactor Applied: {movementFactor}, sprinting={controls.Sprint}");

            if (controls.Jump)
            {
                if (onGround)
                {
                    var blockUnder = this.World.GetBlockAt(this.Player.Position.Minus(Vector3.Down));
                    this.Player.Velocity.Y += 0.42d * (blockUnder.Id == HoneyBlock.BlockId ? PhysicsConst.HoneyblockJumpSpeed : 1);

                    var effectLevel = this.Player.GetEffectLevel(JumpboostEffect.EffectId);
                    if (effectLevel.HasValue)
                    {
                        this.Player.Velocity.Y += 0.1d * effectLevel.Value;
                    }
                }
            }

            var heading = CalculateHeading(controls);
            Logger.Debug2($"Heading: {heading}");
            // TODO: When swimming rotate heading vector
            //       by Pitch radians 

            var strafing = heading.Z * 0.98d;
            var forward = heading.X * 0.98d;

            if (controls.Sneak)
            {
                strafing *= 0.3d;
                forward *= 0.3d;
            }

            heading = ApplyHeading(strafing, forward, movementFactor);
            Logger.Debug2($"Applied heading: {heading}");

            this.Player.Velocity.Add(heading);

            Move(controls, TruncateVector(this.Player.Velocity));

            if (!this.Player.IsOnGround)
            {
                Logger.Debug2($"Applying gravity");
                var gravity = PhysicsConst.Gravity;

                if (this.PlayerState.IsInWater)
                {
                    gravity /= 4.0d;
                }
                this.Player.Velocity.Subtract(new Vector3(0f, gravity, 0f));
            }

            if (this.PlayerState.IsInWater)
            {
                this.Player.Velocity.Mul(new Vector3(slipperiness, slipperiness, slipperiness));
            }
            else
            {
                this.Player.Velocity.Mul(new Vector3(slipperiness, 0.98d, slipperiness));
            }

            this.Player.Velocity = TruncateVector(this.Player.Velocity);

            Logger.Debug2($"Start: Vel={this.Player.Velocity} Pos={this.Player.Position}");
            Logger.Debug2($"---- ---- ---- - End Tick - ---- ---- ----");
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

        public AABB GetPlayerBB(Vector3 pos)
        {
            var w = PhysicsConst.PlayerHalfWidth;
            var bb = new AABB(-w, 0, -w, w, PhysicsConst.PlayerHeight, w)
                .Offset(pos.X, pos.Y, pos.Z);
            return bb;
        }

        private void Move(MovementControls controls, Vector3 amount)
        {
            var wasOnGround = this.Player.IsOnGround;
            Logger.Debug2($"Move by amount: {amount}, wasOnGround={wasOnGround}");

            bool collideY = CheckY(ref amount, false);
            bool collideX = CheckX(ref amount, false);
            bool collideZ = CheckZ(ref amount, false);

            if (!collideX && CheckX(ref amount, true))
            {
                collideX = true;
            }

            if (collideZ && CheckZ(ref amount, true))
            {
                collideZ = true;
            }
            Logger.Debug2($"CollidedX: {collideX} CollidedY: {collideY} CollidedZ: {collideZ}");

            if (controls.Sneak && wasOnGround)
            {
                FixSneaking(ref amount);
            }

            this.Player.Position.Add(amount);
            this.Player.IsOnGround = DetectOnGround();
            Logger.Debug2($"New OnGround: {this.Player.IsOnGround}");
        }

        private bool DetectOnGround()
        {
            Logger.Debug2($"Checking on ground...");
            var entityBoundingBox = GetPlayerBB(this.Player.Position).Offset(0, -PhysicsConst.Gravity, 0);
            Logger.Debug2($"PlayerBB: {entityBoundingBox}");

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
            Logger.Debug2($"Y: {y}");

            for (int x = (int)(Math.Floor(minX)); x <= (int)(Math.Ceiling(maxX)); x++)
            {
                for (int z = (int)(Math.Floor(minZ)); z <= (int)(Math.Ceiling(maxZ)); z++)
                {
                    var block = this.World.GetBlockAt(new Position(x, y, z));

                    if (!block.IsSolid())
                        continue;

                    var coords = new Vector3(x, y, z);

                    foreach (var box in block.GetBlockShape().Select(x => x.ToBoundingBox().Offset(block.Position!.X, block.Position.Y, block.Position.Z)))
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

            var boundingBox = GetPlayerBB(this.Player.Position);

            while (dX != 0.0f && !GetIntersecting(boundingBox.Offset(dX, -PhysicsConst.MaxFallDistance, 0d)).Any())
            {
                if (dX < increment && dX >= -increment)
                    dX = 0.0f;
                else if (dX > 0.0D)
                    dX -= increment;
                else
                    dX += increment;

                correctedX = dX;
            }

            while (dZ != 0.0f && !GetIntersecting(boundingBox.Offset(0d, -PhysicsConst.MaxFallDistance, dZ)).Any())
            {
                if (dZ < increment && dZ >= -increment)
                    dZ = 0.0f;
                else if (dZ > 0.0f)
                    dZ -= increment;
                else
                    dZ += increment;

                correctedZ = dZ;
            }

            while (dX != 0.0f && dZ != 0.0f && !GetIntersecting(boundingBox.Offset(dX, -PhysicsConst.MaxFallDistance, dZ)).Any())
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
            var minX = (int)Math.Floor(box.MinY);
            var maxX = (int)Math.Ceiling(box.MaxX);

            var minZ = (int)Math.Floor(box.MinZ);
            var maxZ = (int)Math.Ceiling(box.MaxZ);

            var minY = (int)Math.Floor(box.MinY);
            var maxY = (int)Math.Ceiling(box.MaxY);

            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    for (int z = minZ; z < maxZ; z++)
                    {
                        var coords = new Vector3(x, y, z);

                        var block = this.World.GetBlockAt(new Position(x, y, z));

                        if (block == null)
                            continue;

                        if (!block.IsSolid())
                            continue;

                        foreach (var blockBox in block.GetBlockShape().Select(x => x.ToBoundingBox().Offset(block.Position!.X, block.Position.Y, block.Position.Z)))
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

            if (!TestTerrainCollisionY(ref amount, out var yCollisionPoint, out var collisionY))
                return false;

            var yVelocity = CollidedWithWorld(beforeAdjustment < 0 ? Vector3.Down : Vector3.Up, yCollisionPoint, beforeAdjustment);

            if (MathF.Abs(yVelocity) < 0.005f)
                yVelocity = 0;

            amount.Y = collisionY;

            this.Player.Velocity = new Vector3(this.Player.Velocity.X, yVelocity, this.Player.Velocity.Z);

            return true;
        }

        private bool CheckX(ref Vector3 amount, bool checkOther)
        {
            if (!TestTerrainCollisionX(amount, out _, out var collisionX, checkOther))
                return false;

            var climbingResult = CheckClimbing(amount, out double yValue);
            if (climbingResult == CollisionResult.ClimbHalfBlock)
            {
                amount.Y = yValue;
            }
            else
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
            if (!TestTerrainCollisionZ(amount, out _, out var collisionZ, checkOther))
                return false;

            var climbingResult = CheckClimbing(amount, out double yValue);
            if (climbingResult == CollisionResult.ClimbHalfBlock)
            {
                amount.Y = yValue;
            }
            else
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

            var entityBox = GetPlayerBB(this.Player.Position);
            AABB testBox;

            if (velocity.Y < 0)
            {
                testBox = entityBox.Clone();
                testBox.MinY += velocity.Y;

                negative = true;
            }
            else
            {
                testBox = entityBox.Clone();
                testBox.MaxY += velocity.Y;

                negative = false;
            }

            double? collisionExtent = null;

            for (int x = (int)(Math.Floor(testBox.MinX)); x <= (int)(Math.Ceiling(testBox.MaxX)); x++)
            {
                for (int z = (int)(Math.Floor(testBox.MinZ)); z <= (int)(Math.Ceiling(testBox.MaxZ)); z++)
                {
                    for (int y = (int)(Math.Floor(testBox.MinY)); y <= (int)(Math.Ceiling(testBox.MaxY)); y++)
                    {
                        var blockState = this.World.GetBlockAt(new Position(x, y, z));

                        if (!blockState.IsSolid())
                            continue;

                        var coords = new Vector3(x, y, z);

                        foreach (var box in blockState.GetBlockShape().Select(c => c.ToBoundingBox().Offset(x, y, z)))
                        {
                            if (negative)
                            {
                                if (entityBox.MinY - box.MaxY < 0)
                                    continue;
                            }
                            else
                            {
                                if (box.MinY - entityBox.MaxY < 0)
                                    continue;
                            }

                            if (testBox.Intersects(box))
                            {
                                if (negative)
                                {
                                    if ((collisionExtent == null || collisionExtent.Value < box.MaxY))
                                    {
                                        collisionExtent = box.MaxY;
                                        collisionPoint = coords;
                                    }
                                }
                                else
                                {
                                    if ((collisionExtent == null || collisionExtent.Value > box.MinY))
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

            var entityBox = GetPlayerBB(this.Player.Position);
            AABB testBox;

            Vector3 min = new Vector3(entityBox.MinX, entityBox.MinY, entityBox.MinZ);
            Vector3 max = new Vector3(entityBox.MaxX, entityBox.MaxY, entityBox.MaxZ);

            if (velocity.X < 0)
            {
                min.X += velocity.X;
                negative = true;
            }
            else
            {
                max.X += velocity.X;
                negative = false;
            }

            if (includeOther)
            {
                if (velocity.Z < 0)
                {
                    min.Z += velocity.Z;
                }
                else
                {
                    max.Z += velocity.Z;
                }

                if (velocity.Y < 0)
                {
                    min.Y += velocity.Y;
                }
                else
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

            for (int x = (int)(Math.Floor(minX)); x <= (int)(Math.Ceiling(maxX)); x++)
            {
                for (int z = (int)(Math.Floor(minZ)); z <= (int)(Math.Ceiling(maxZ)); z++)
                {
                    for (int y = (int)(Math.Floor(minY)); y <= (int)(Math.Ceiling(maxY)); y++)
                    {
                        var blockState = this.World.GetBlockAt(new Position(x, y, z));

                        if (!blockState.IsSolid())
                            continue;

                        var coords = new Vector3(x, y, z);

                        foreach (var box in blockState.GetBlockShape().Select(x => x.ToBoundingBox().Offset(blockState.Position!.X, blockState.Position.Y, blockState.Position.Z)))
                        {
                            if (box.MaxY <= minY) continue;

                            if (negative)
                            {
                                if (box.MaxX <= minX)
                                    continue;

                                if (entityBox.MinX - box.MaxX < 0)
                                    continue;
                            }
                            else
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
                                    if ((collisionExtent == null || collisionExtent.Value < box.MaxX))
                                    {
                                        collisionExtent = box.MaxX;
                                        collisionPoint = coords;
                                    }
                                }
                                else
                                {
                                    if ((collisionExtent == null || collisionExtent.Value > box.MinX))
                                    {
                                        collisionExtent = box.MinX;
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
                double diff;

                if (negative)
                {
                    diff = (collisionExtent.Value - minX) + 0.01f;
                }
                else
                {
                    diff = (collisionExtent.Value - maxX);
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

            var entityBox = GetPlayerBB(this.Player.Position);
            AABB testBox;

            Vector3 min = new Vector3(entityBox.MinX, entityBox.MinY, entityBox.MinZ);
            Vector3 max = new Vector3(entityBox.MaxX, entityBox.MaxY, entityBox.MaxZ);

            if (velocity.Z < 0)
            {
                min.Z += velocity.Z;
                negative = true;
            }
            else
            {
                max.Z += velocity.Z;
                negative = false;
            }

            if (includeOther)
            {
                if (velocity.X < 0)
                {
                    min.X += velocity.X;
                }
                else
                {
                    max.X += velocity.X;
                }

                if (velocity.Y < 0)
                {
                    min.Y += velocity.Y;
                }
                else
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

            for (int x = (int)(Math.Floor(minX)); x <= (int)(Math.Ceiling(maxX)); x++)
            {
                for (int z = (int)(Math.Floor(minZ)); z <= (int)(Math.Ceiling(maxZ)); z++)
                {
                    for (int y = (int)(Math.Floor(minY)); y <= (int)(Math.Ceiling(maxY)); y++)
                    {
                        var blockState = this.World.GetBlockAt(new Position(x, y, z));

                        if (!blockState.IsSolid())
                            continue;

                        var coords = new Vector3(x, y, z);

                        foreach (var box in blockState.GetBlockShape().Select(x => x.ToBoundingBox().Offset(blockState.Position!.X, blockState.Position.Y, blockState.Position.Z)))
                        {
                            if (box.MaxY <= minY) continue;

                            if (negative)
                            {
                                if (box.MaxZ <= minZ)
                                    continue;

                                if (entityBox.MinZ - box.MaxZ < 0)
                                    continue;
                            }
                            else
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
                                    if ((collisionExtent == null || collisionExtent.Value < box.MaxZ))
                                    {
                                        collisionExtent = box.MaxZ;
                                        collisionPoint = coords;
                                    }
                                }
                                else
                                {
                                    if ((collisionExtent == null || collisionExtent.Value > box.MinZ))
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
                    diff = (cp - minZ) + 0.01f;
                else
                    diff = (cp - maxZ);

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
                var adjusted = GetPlayerBB(this.Player.Position.Plus(amount));
                var intersecting = GetIntersecting(adjusted);
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
                    adjusted = GetPlayerBB(this.Player.Position.Plus(new Vector3(amount.X, targetY, amount.Z)));

                    if (GetIntersecting(adjusted).Any(
                            bb => bb.MaxY > adjusted.MinY && bb.MinY <= adjusted.MaxY))
                    {
                        canJump = false;
                    }
                }
                else
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

        internal enum CollisionResult
        {
            DoNothing,
            ClimbHalfBlock,
            VerticalClimb
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
    }
}
