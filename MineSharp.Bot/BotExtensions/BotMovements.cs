using MineSharp.Core.Types;
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

        private partial void LoadMovements() {
        }

        public async Task ForceSetRotation(float yaw, float pitch, CancellationToken? cancellation = null) {
            var packet = new Protocol.Packets.Serverbound.Play.PlayerRotationPacket(yaw, pitch, BotEntity.IsOnGround);
            
            await Client.SendPacket(packet, cancellation);

            if (cancellation.HasValue && cancellation.Value.IsCancellationRequested) return;
            this.BotEntity.Yaw = yaw;
            this.BotEntity.Pitch = pitch;
            this.BotMoved?.Invoke(BotEntity);
        }

        public Task ForceLookAt(Position position, CancellationToken? cancellation) { 
            float dx = position.X - (float)this.BotEntity.Position.X;
            float dy = position.Y - (float)this.BotEntity.Position.Y;
            float dz = position.Z - (float)this.BotEntity.Position.Z;

            float r = MathF.Sqrt(dx * dx + dy * dy + dz * dz);
            float yaw = -MathF.Atan2(dx, dz) / MathF.PI * 180;
            if (yaw < 0) yaw = 360 + yaw;
            float pitch = -MathF.Asin(dy / r) / MathF.PI * 180;
            return ForceSetRotation(yaw, pitch, cancellation);
        }

    }
}
