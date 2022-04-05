using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class FacePlayerPacket : Packet {

        public AimLocation AimPosition { get; private set; }
        public double TargetX { get; private set; }
        public double TargetY { get; private set; }
        public double TargetZ { get; private set; }
        public bool IsEntity { get; private set; }
        public int? EntityId { get; private set; }
        public AimLocation EntityAimAt { get; private set; }


        public FacePlayerPacket() { }

        public FacePlayerPacket(AimLocation aimPosition, double targetX, double targetY, double targetZ, bool isEntity, int? entityId, AimLocation entityAimAt) {
            AimPosition = aimPosition;
            TargetX = targetX;
            TargetY = targetY;
            TargetZ = targetZ;
            IsEntity = isEntity;
            EntityId = entityId;
            EntityAimAt = entityAimAt;
        }

        public override void Read(PacketBuffer buffer) {
            this.AimPosition = (AimLocation)buffer.ReadVarInt();
            this.TargetX = buffer.ReadDouble();
            this.TargetY = buffer.ReadDouble();
            this.TargetZ = buffer.ReadDouble();
            this.IsEntity = buffer.ReadBoolean();
            if (this.IsEntity) {
                this.EntityId = buffer.ReadVarInt();
                this.EntityAimAt = (AimLocation) buffer.ReadVarInt();
            }
        }

        public override void Write(PacketBuffer buffer) {
            throw new NotImplementedException();
        }

        public enum AimLocation {
            Feet = 0,
            Eyes = 1
        }
    }
}
