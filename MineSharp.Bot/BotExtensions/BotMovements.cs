using MineSharp.Core.Types;
using MineSharp.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Bot {
    public partial class Bot {

        /// <summary>
        /// Fires when the Bot <see cref="BotEntity"/> moves
        /// </summary>
        public BotPlayerEvent BotMoved;

        public bool PhysicsEnabled { get; set; } = true;
        private Task PhysicsLoop;
        public Physics.Physics Physics;

        public PlayerControls MovementControls = new PlayerControls();

        private partial void LoadMovements() {
            void Bot_Joined() {
                Physics = new Physics.Physics(this.BotEntity, this.World);
                LastSentPlayerInfo = new PlayerInfoState() { Pos = BotEntity.Position.Clone(), Pitch = BotEntity.Pitch, Yaw = BotEntity.Yaw, IsOnGround = BotEntity.IsOnGround };
                PhysicsLoop = Task.Run(DoPhysics);
            }
            Joined += Bot_Joined;

        }

        private async void DoPhysics() {

            while (World.Chunks.Count < 100) await Task.Delay(10); //TODO:
            while (true) {
                try {
                    if (PhysicsEnabled) 
                        Physics.SimulatePlayer(MovementControls);
                    
                    UpdatePositionOrLookIfNecessary();
                } catch (Exception e) { Logger.Error("Error in PhysicsLoop: " + e.ToString()); }
                await Task.Delay(50);
            }
        }

        private struct PlayerInfoState {
            public Vector3 Pos;
            public float Yaw;
            public float Pitch;
            public bool IsOnGround;

            public override string ToString() {
                return $"Pos={Pos} Yaw={Yaw} Pitch={Pitch} IsOnGround={IsOnGround}";
            }
        }

        private PlayerInfoState LastSentPlayerInfo = new PlayerInfoState();


        private void UpdatePositionOrLookIfNecessary() {
            if (LastSentPlayerInfo.Pos.X != BotEntity.Position.X || 
                LastSentPlayerInfo.Pos.Y != BotEntity.Position.Y || 
                LastSentPlayerInfo.Pos.Z != BotEntity.Position.Z || 
                LastSentPlayerInfo.Yaw != BotEntity.Yaw || 
                LastSentPlayerInfo.Pitch != BotEntity.Pitch || 
                LastSentPlayerInfo.IsOnGround != BotEntity.IsOnGround) {
                UpdateServerPositionAndLook();
            }
        }

        private Task UpdateServerPositionAndLook() {
            var packet = new Protocol.Packets.Serverbound.Play.PlayerPositionAndRotationPacket(BotEntity.Position.X, BotEntity.Position.Y, BotEntity.Position.Z, BotEntity.Yaw, BotEntity.Pitch, BotEntity.IsOnGround);
            LastSentPlayerInfo.Pos.X = packet.X;
            LastSentPlayerInfo.Pos.Y = packet.FeetY;
            LastSentPlayerInfo.Pos.Z = packet.Z;
            LastSentPlayerInfo.Yaw = packet.Yaw;
            LastSentPlayerInfo.Pitch = packet.Pitch;
            BotMoved?.Invoke(BotEntity);
            return Client.SendPacket(packet);
        }

        public void ForceSetRotation(float yaw, float pitch) {
            this.BotEntity.Yaw = yaw;
            this.BotEntity.Pitch = pitch;
            this.BotMoved?.Invoke(BotEntity);
        }

        public void ForceLookAt(Position position) {
            var playerPos = this.BotEntity.GetHeadPosition();
            float dx = position.X - (float)playerPos.X;
            float dy = position.Y - (float)playerPos.Y;
            float dz = position.Z - (float)playerPos.Z;

            float r = MathF.Sqrt(dx * dx + dy * dy + dz * dz);
            float yaw = -MathF.Atan2(dx, dz) / MathF.PI * 180;
            if (yaw < 0) yaw = 360 + yaw;
            float pitch = -MathF.Asin(dy / r) / MathF.PI * 180;
            ForceSetRotation(yaw, pitch);
        }

    }
}
