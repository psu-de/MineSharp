using MineSharp.Core.Types;
using MineSharp.Data.Entities;
using MineSharp.Data.Blocks;
using MineSharp.Data.Effects;

namespace MineSharp.Physics {
    public class Physics {

        public Player Player;
        public PlayerState PlayerState;
        public World.World World;
        public Physics(Player player, World.World world) {
            this.World = world;
            this.Player = player;
            this.PlayerState = new PlayerState(player);
        }

        public List<AABB> GetSurroundingBoundingBoxes(AABB queryBB) {
            List<AABB> surroundingBBs = new List<AABB>();
            var cursor = new Vector3(0, 0, 0);
            for (cursor.Y = Math.Floor(queryBB.MinY) - 1; cursor.Y <= Math.Floor(queryBB.MaxY); cursor.Y++) {
                for (cursor.Z = Math.Floor(queryBB.MinZ); cursor.Z <= Math.Floor(queryBB.MaxZ); cursor.Z++) {
                    for (cursor.X = Math.Floor(queryBB.MinX); cursor.X <= Math.Floor(queryBB.MaxX); cursor.X++) {
                        var block = this.World.GetBlockAt(cursor);
                        if (block.IsSolid()) {
                            var blockPos = block.Position;
                            foreach (var shape in block.GetBlockShape()) { //TODO: Block Shapes!
                                var blockBB = shape.ToBoundingBox();
                                blockBB.Offset(blockPos!.X, blockPos.Y, blockPos.Z);
                                surroundingBBs.Add(blockBB);
                            }
                        }
                    }
                }
            }

            return surroundingBBs;
        }

        public void AdjustPositionHeight(ref Vector3 position) {
            var playerBB = PhysicsConst.GetPlayerBoundingBox(position);
            var queryBB = playerBB.Clone().Extend(0, -1, 0);
            var surroundingBBs = GetSurroundingBoundingBoxes(queryBB);

            double dy = -1;
            foreach (var blockBB in surroundingBBs) {
                dy = blockBB.ComputeOffsetY(playerBB, dy);
            }
            position.Y += dy;
        }

        public void SetPositionToBoundingBox(AABB bb, Vector3 position) {
            position.X = bb.MinX + PhysicsConst.PlayerHalfWidth;
            position.Y = bb.MinY;
            position.Z = bb.MinZ + PhysicsConst.PlayerHalfWidth;
        }

        private void MoveEntity(double dx, double dy, double dz, PlayerControls controls) {
            var vel = this.Player.Velocity;
            var pos = this.Player.Position;

            if (this.PlayerState.IsInWeb) {
                dx *= 0.25f;
                dy *= 0.25f;
                dz *= 0.25f;
                vel.X = 0;
                vel.Y = 0;
                vel.Z = 0;
                this.PlayerState.IsInWeb = false;
            }

            var oldVelX = dx;
            var oldVelY = dy;
            var oldVelZ = dz;
            
            if (controls.Sneak && Player.IsOnGround) {
                var step = 0.05;

                // In the 3 loops bellow, y offset should be -1, but that doesnt reproduce vanilla behavior.
                for (; dx != 0 && GetSurroundingBoundingBoxes(PhysicsConst.GetPlayerBoundingBox(pos).Offset(dx, 0, 0)).Count == 0; oldVelX = dx) {
                    if (dx < step && dx >= -step) dx = 0;
                    else if (dx > 0) dx -= step;
                    else dx += step;
                }

                for (; dz != 0 && GetSurroundingBoundingBoxes(PhysicsConst.GetPlayerBoundingBox(pos).Offset(0, 0, dz)).Count == 0; oldVelZ = dz) {
                    if (dz < step && dz >= -step) dz = 0;
                    else if (dz > 0) dz -= step;
                    else dz += step;
                }

                while (dx != 0 && dz != 0 && GetSurroundingBoundingBoxes(PhysicsConst.GetPlayerBoundingBox(pos).Offset(dx, 0, dz)).Count == 0) {
                    if (dx < step && dx >= -step) dx = 0;
                    else if (dx > 0) dx -= step;
                    else dx += step;


                    if (dz < step && dz >= -step) dz = 0;
                    else if (dz > 0) dz -= step;
                    else dz += step;

                    oldVelX = dx;
                    oldVelZ = dz;
                }
            }

            var playerBB = PhysicsConst.GetPlayerBoundingBox(pos);
            var queryBB = playerBB.Clone().Extend(dx, dy, dz);
            var surroudingBBs = GetSurroundingBoundingBoxes(queryBB);
            var oldBB = playerBB.Clone();

            foreach (var bb in surroudingBBs) {
                dy = bb.ComputeOffsetY(playerBB, dy);
            }
            playerBB.Offset(0, dy, 0);

            foreach (var bb in surroudingBBs) {
                dx = bb.ComputeOffsetX(playerBB, dx);
            }
            playerBB.Offset(dx, 0, 0);


            foreach (var bb in surroudingBBs) {
                dz = bb.ComputeOffsetZ(playerBB, dz);
            }
            playerBB.Offset(0, 0, dz);


            if (PhysicsConst.StepHeight > 0 &&
                (this.Player.IsOnGround || (dy != oldVelY && oldVelY < 0)) &&
                (dx != oldVelX || dz != oldVelZ)) {
                var oldVelXCol = dx;
                var oldVelYCol = dy;
                var oldVelZCol = dz;
                var oldBBCol = playerBB.Clone();

                dy = PhysicsConst.StepHeight;
                queryBB = oldBB.Clone().Extend(oldVelX, dy, oldVelZ);
                var surroundingBBs = GetSurroundingBoundingBoxes(queryBB);

                var BB1 = oldBB.Clone();
                var BB2 = oldBB.Clone();
                var BB_XZ = BB1.Clone().Extend(dx, 0, dz);

                var dy1 = dy;
                var dy2 = dy;
                foreach (var blockBB in surroudingBBs) {
                    dy1 = blockBB.ComputeOffsetY(BB_XZ, dy1);
                    dy2 = blockBB.ComputeOffsetY(BB2, dy2);
                }
                BB1.Offset(0, dy1, 0);
                BB2.Offset(0, dy2, 0);

                var dx1 = oldVelX;
                var dx2 = oldVelX;
                foreach (var blockBB in surroundingBBs) {
                    dx1 = blockBB.ComputeOffsetX(BB1, dx1);
                    dx2 = blockBB.ComputeOffsetX(BB2, dx2);
                }
                BB1.Offset(dx1, 0, 0);
                BB2.Offset(dx2, 0, 0);

                var dz1 = oldVelZ;
                var dz2 = oldVelZ;
                foreach (var blockBB in surroundingBBs) {
                    dz1 = blockBB.ComputeOffsetZ(BB1, dz1);
                    dz2 = blockBB.ComputeOffsetZ(BB2, dz2);
                }
                BB1.Offset(0, 0, dz1);
                BB2.Offset(0, 0, dz2);

                var norm1 = dx1 * dx1 + dz1 * dz1;
                var norm2 = dx2 * dx2 + dz2 * dz2;

                if (norm1 > norm2) {
                    dx = dx1;
                    dy = -dy1;
                    dz = dz1;
                    playerBB = BB1;
                } else {
                    dx = dx2;
                    dy = -dy2;
                    dz = dz2;
                    playerBB = BB2;
                }

                foreach (var blockBB in surroundingBBs) {
                    dy = blockBB.ComputeOffsetY(playerBB, dy);
                }
                playerBB.Offset(0, dy, 0);

                if (oldVelXCol * oldVelXCol + oldVelZCol * oldVelZCol >= dx * dx + dz * dz) {
                    dx = oldVelXCol;
                    dy = oldVelYCol;
                    dz = oldVelZCol;
                    playerBB = oldBBCol;
                }
            }

            SetPositionToBoundingBox(playerBB, pos);
            this.PlayerState.IsCollidedHorizontally = dx != oldVelX || dz != oldVelZ;
            this.PlayerState.IsCollidedVertically = dy != oldVelY;
            this.Player.IsOnGround = this.PlayerState.IsCollidedVertically && oldVelY < 0;

            var blockAtFeet = this.World.GetBlockAt(pos.Floored().Plus(Vector3.Down));

            if (dx != oldVelX) vel.X = 0;
            if (dz != oldVelZ) vel.Y = 0;
            if (dy != oldVelY) {
                if (blockAtFeet.IsSolid() && blockAtFeet.Id == SlimeBlock.BlockId && !controls.Sneak) {
                    vel.Y = -vel.Y;
                } else {
                    vel.Y = 0;
                }
            }

            playerBB.Contract(0.001, 0.001, 0.001);
            var cursor = new Vector3(0, 0, 0);
            for (cursor.Y = Math.Floor(playerBB.MinY); cursor.Y <= Math.Floor(playerBB.MaxY); cursor.Y++) {
                for (cursor.Z = Math.Floor(playerBB.MinZ); cursor.Z <= Math.Floor(playerBB.MaxZ); cursor.Z++) {
                    for (cursor.X = Math.Floor(playerBB.MinX); cursor.X <= Math.Floor(playerBB.MaxX); cursor.X++) {
                        var block = this.World.GetBlockAt(cursor);
                        if (block.IsSolid()) {

                            if (block.Id == Cobweb.BlockId) {
                                this.PlayerState.IsInWeb = true;
                            } else if (block.Id == BubbleColumn.BlockId) {
                                var down = block.Metadata == 0;
                                var aboveBlock = this.World.GetBlockAt(cursor.Plus(Vector3.Up));
                                var bubbleDrag = (!aboveBlock.IsSolid()) ? PhysicsConst.BubbleColumnSurfaceDrag : PhysicsConst.BubbleColumnDrag;
                                if (down) {
                                    vel.Y = Math.Max(bubbleDrag.MaxDown, vel.Y - bubbleDrag.Down);
                                } else {
                                    vel.Y = Math.Min(bubbleDrag.MaxUp, vel.Y + bubbleDrag.Up);
                                }
                            }
                        }
                    }
                }
            }

            var blockBelow = this.World.GetBlockAt(this.Player.Position.Floored().Plus(Vector3.Down));
            if (blockBelow.IsSolid()) {
                if (blockBelow.Id == SoulSand.BlockId) {
                    vel.X *= PhysicsConst.SoulsandSpeed;
                    vel.Z *= PhysicsConst.SoulsandSpeed;
                } else if (blockBelow.Id == HoneyBlock.BlockId) {
                    vel.X *= PhysicsConst.HoneyblockSpeed;
                    vel.Z *= PhysicsConst.HoneyblockSpeed;
                }
            }

            this.Player.Velocity = vel;
        }

        public void ApplyHeading(double strafe, double forward, double multiplier) {
            var speed = Math.Sqrt(strafe * strafe + forward * forward);
            if (speed < 0.01) return;

            speed = multiplier / Math.Max(speed, 1);

            strafe *= speed;
            forward *= speed;

            var yaw = Math.PI - this.Player.YawRadians;
            var sin = Math.Sin(yaw);
            var cos = Math.Cos(yaw);

            this.Player.Velocity.X += strafe * cos - forward * sin;
            this.Player.Velocity.Z += forward * cos + strafe * sin;
        }

        public bool IsOnLadder(Vector3 pos) {
            var block = this.World.GetBlockAt(pos.Floored());
            return (block.IsSolid() && (block.Id == Ladder.BlockId || block.Id == Vine.BlockId)); // TODO: Other vines?
        }

        public bool DoesNotCollide(Vector3 pos) {
            var pBB = PhysicsConst.GetPlayerBoundingBox(pos);
            return !GetSurroundingBoundingBoxes(pBB).Any(x => pBB.Intersects(x)) && GetWaterInBB(pBB).Count == 0;
        }

        public int GetRenderedDepth(Block block) {
            if (!block.IsSolid()) return -1;
            if (PhysicsConst.WaterLikeBlocks.Contains(block.Id)) return 0;
            if (block.Properties.Get("waterlogged")?.GetValue<bool>() == true) return 0;
            if (block.Id != Water.BlockId) return -1;

            return block.Metadata >= 8 ? 0 : block.Metadata;
        }


        public int GetLiquidHeightPercent(Block block) {
            return (GetRenderedDepth(block) + 1) / 9;
        }

        public List<Block> GetWaterInBB(AABB bb) {
            var waterBlocks = new List<Block>();
            var cursor = new Vector3(0, 0, 0);
            for (cursor.Y = Math.Floor(bb.MinY); cursor.Y <= Math.Floor(bb.MaxY); cursor.Y++) {
                for (cursor.Z = Math.Floor(bb.MinZ); cursor.Z <= Math.Floor(bb.MaxZ); cursor.Z++) {
                    for (cursor.X = Math.Floor(bb.MinX); cursor.X <= Math.Floor(bb.MaxX); cursor.X++) {
                        var block = this.World.GetBlockAt(cursor);
                        if (block.IsSolid() && (block.Id == Water.BlockId || PhysicsConst.WaterLikeBlocks.Contains(block.Id) || block.Properties.Get("waterlogged")?.GetValue<bool>() == true)) {
                            var waterLevel = cursor.Y + 1 - GetLiquidHeightPercent(block);
                            if (Math.Ceiling(bb.MaxY) >= waterLevel) waterBlocks.Add(block);
                        }
                    }
                }
            }
            return waterBlocks;
        }

        public void MoveEntityWithHeading(double strafe, double forward, PlayerControls controls) {
            var vel = this.Player.Velocity;
            var pos = this.Player.Position;

            var gravityMultiplier = (vel.Y <= 0 && this.PlayerState.SlowFalling > 0) ? PhysicsConst.SlowFalling : 1;

            if (!this.PlayerState.IsInWater && !this.PlayerState.IsInLava) {
                // Normal movement
                var acceleration = PhysicsConst.AirborneAcceleration;
                var inertia = PhysicsConst.AirborneInertia;
                var blockUnder = this.World.GetBlockAt(pos.Floored().Plus(Vector3.Down));

                if (this.Player.IsOnGround && blockUnder.IsSolid()) {
                    inertia = PhysicsConst.GetBlockSlipperiness(blockUnder.Id) * 0.91;
                    acceleration = 0.1 * (0.1627714 / (inertia * inertia * inertia));
                }

                if (controls.Sprint) acceleration *= PhysicsConst.SprintSpeed;

                var speedLevel = Player.GetEffectLevel(SpeedEffect.EffectId);
                var slownessLevel = Player.GetEffectLevel(SlownessEffect.EffectId);
                if (speedLevel != null && speedLevel > 0) acceleration *= PhysicsConst.SpeedEffect * (int)speedLevel;
                if (slownessLevel != null && slownessLevel > 0) acceleration *= PhysicsConst.SlowEffect * (int)slownessLevel;

                ApplyHeading(strafe, forward, acceleration);

                if (IsOnLadder(pos)) {
                    vel.X = Math.Clamp(-PhysicsConst.LadderMaxSpeed, vel.X, PhysicsConst.LadderMaxSpeed);
                    vel.Z = Math.Clamp(-PhysicsConst.LadderMaxSpeed, vel.Z, PhysicsConst.LadderMaxSpeed);
                    vel.Y = Math.Max(vel.Y, (controls.Sneak ? 1 : 0) -PhysicsConst.LadderMaxSpeed);
                }

                MoveEntity(vel.X, vel.Y, vel.Z, controls);

                if (IsOnLadder(pos) && (this.PlayerState.IsCollidedHorizontally || controls.Jump)) {
                    vel.Y = PhysicsConst.LadderClimbSpeed; // climb ladder
                }

                // Apply friction and gravity
                if (this.PlayerState.Levitation > 0) {
                    vel.Y += (0.05 * this.PlayerState.Levitation - vel.Y) * 0.2;
                } else {
                    vel.Y -= PhysicsConst.Gravity * gravityMultiplier;
                }
                vel.Y *= PhysicsConst.Airdrag;
                vel.X *= inertia;
                vel.Z *= inertia;
            } else {
                // Water / Lava movement
                var lastY = pos.Y;
                var acceleration = PhysicsConst.LiquidAcceleration;
                var inertia = this.PlayerState.IsInWater ? PhysicsConst.WaterInertia : PhysicsConst.LavaInertia;
                var horizontalInertia = inertia;

                if (this.PlayerState.IsInWater) {
                    float strider = Math.Min(this.PlayerState.DepthStrider, 3);
                    if (!this.Player.IsOnGround) {
                        strider *= 0.5f;
                    }
                    if (strider > 0) {
                        horizontalInertia += (0.546 - horizontalInertia) * strider / 3;
                        acceleration += (0.7 - acceleration) * strider / 3;
                    }

                    if (this.PlayerState.DolphinsGrace > 0) horizontalInertia = 0.96;
                }

                ApplyHeading(strafe, forward, acceleration);
                MoveEntity(vel.X, vel.Y, vel.Z, controls);
                vel.Y *= inertia;
                vel.Y -= (this.PlayerState.IsInWater ? PhysicsConst.WaterGravity : PhysicsConst.LavaGravity) * gravityMultiplier;
                vel.X *= horizontalInertia;
                vel.Z *= horizontalInertia;

                if (this.PlayerState.IsCollidedHorizontally && DoesNotCollide(pos.Plus(new(vel.X, vel.Y + 0.6 - pos.Y + lastY, vel.Z)))) {
                    vel.Y = PhysicsConst.OutOfLiquidImpulse; // jump out of liquid
                }
            }

            this.Player.Velocity = vel;
        }

        public Vector3 GetFlow(Block block) {
            var curlevel = GetRenderedDepth(block);
            var flow = new Vector3(0, 0, 0);
            foreach ((int dx, int dz) in new[] { (0, 1), (-1, 0), (0, -1), (1, 0) }) {
                var adjBlock = this.World.GetBlockAt(((Vector3)block.Position!).Plus(new Vector3(dx, 0, dz)));
                var adjLevel = GetRenderedDepth(adjBlock);
                if (adjLevel < 0) {
                    if (adjBlock.IsSolid() && adjBlock.BoundingBox != "empty") {
                        adjLevel = GetRenderedDepth(this.World.GetBlockAt(((Vector3)block.Position).Plus(new Vector3(dx, -1, dz))));
                        if (adjLevel >= 0) {
                            var f = adjLevel - (curlevel - 8);
                            flow.X += dx * f;
                            flow.Z += dz * f;
                        }
                    }
                } else {
                    var f = adjLevel - curlevel;
                    flow.X += dx * f;
                    flow.Z += dz * f;
                }
            }

            if (block.Metadata >= 8) {
                foreach ((int dx, int dz) in new [] { (0, 1), (-1, 0), (0, -1), (1, 0) }) {

                    var bPos1 = new Position(block.Position!.X + dx, block.Position.Y, block.Position.Z + dz);
                    var bPos2 = new Position(block.Position.X + dx, block.Position.Y + 1, block.Position.Z + dz);

                    var adjBlock = this.World.GetBlockAt(bPos1);
                    var adjUpBlock = this.World.GetBlockAt(bPos2);

                 if ((adjBlock.IsSolid() && adjBlock.BoundingBox != "empty") || (adjUpBlock.IsSolid() && adjUpBlock.BoundingBox != "empty")) {
                        flow.Normalized().Plus(Vector3.Down * 6);
        }
                }
            }


            return flow.Normalized();
        }

        public bool IsInWaterApplyCurrent(AABB bb, Vector3 vel) {
            var acceleration = new Vector3(0, 0, 0);
            var waterBlocks = GetWaterInBB(bb);
            var isInWater = waterBlocks.Count > 0;
            foreach (var block in waterBlocks) {
                var flow = GetFlow(block);
                acceleration.Add(flow);
            }

            var len = acceleration.Length();
            if (len > 0) {
                vel.X += acceleration.X / len * 0.014;
                vel.Y += acceleration.Y / len * 0.014;
                vel.Z += acceleration.Z / len * 0.014;
            }
            return isInWater;
        }

        public bool IsMaterialInBB(AABB queryBB, int type) {
            var cursor = new Vector3(0, 0, 0);
            for (cursor.Y = Math.Floor(queryBB.MinY); cursor.Y <= Math.Floor(queryBB.MaxY); cursor.Y++) {
                for (cursor.Z = Math.Floor(queryBB.MinZ); cursor.Z <= Math.Floor(queryBB.MaxZ); cursor.Z++) {
                    for (cursor.X = Math.Floor(queryBB.MinX); cursor.X <= Math.Floor(queryBB.MaxX); cursor.X++) {
                        var block = this.World.GetBlockAt(cursor);
                        if (block.IsSolid() && block.Id == type) return true;
                    }
                }
            }
            return false;
        }

        public void SimulatePlayer(PlayerControls controls) {
            var vel = this.Player.Velocity;
            var pos = this.Player.Position;

            var waterBB = PhysicsConst.GetPlayerBoundingBox(pos).Contract(0.001, 0.401, 0.001);
            var lavaBB = PhysicsConst.GetPlayerBoundingBox(pos).Contract(0.1, 0.4, 0.1);

            this.PlayerState.IsInWater = IsInWaterApplyCurrent(waterBB, vel);
            this.PlayerState.IsInLava = IsMaterialInBB(lavaBB, Lava.BlockId);

            // Reset velocity component if it falls under the threshold
            if (Math.Abs(vel.X) < PhysicsConst.NegligeableVelocity) vel.X = 0;
            if (Math.Abs(vel.Y) < PhysicsConst.NegligeableVelocity) vel.Y = 0;
            if (Math.Abs(vel.Z) < PhysicsConst.NegligeableVelocity) vel.Z = 0;

            if (controls.Jump || PlayerState.JumpQueued) {
                if (PlayerState.JumpTicks > 0) PlayerState.JumpTicks--;
                if (PlayerState.IsInWater || PlayerState.IsInLava) {
                    vel.Y += 0.04;
                } else if (Player.IsOnGround && PlayerState.JumpTicks == 0) {
                    var blockBelow = this.World.GetBlockAt(Player.Position.Floored().Plus(Vector3.Down));
                    vel.Y = MathF.Round(0.42f) * ((blockBelow.Id == HoneyBlock.BlockId) ? PhysicsConst.HoneyblockJumpSpeed : 1);
                          if (PlayerState.JumpBoost > 0) {
                        vel.Y += 0.1 * PlayerState.JumpBoost;
                    }
                    if (controls.Sprint) {
                        var yaw = Math.PI - Player.YawRadians;
                        vel.X -= Math.Sin(yaw) * 0.2;
                        vel.Z += Math.Cos(yaw) * 0.2;
                    }
                    PlayerState.JumpTicks = PhysicsConst.AutoJumpCooldown;
                }
            } else {
                PlayerState.JumpTicks = 0; // reset autojump cooldown
            }
            this.PlayerState.JumpQueued = false;

            var strafe = ((controls.Right ? 1f : 0f) - (controls.Left ? 1f : 0f)) * 0.98;
            var forward = ((controls.Forward ? 1f : 0f) - (controls.Back ? 1f : 0f)) * 0.98;

            if (controls.Sneak) {
                strafe *= PhysicsConst.SneakSpeed;
                forward *= PhysicsConst.SneakSpeed;
            }


            MoveEntityWithHeading(strafe, forward, controls);
        }
    }
}
