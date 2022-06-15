using MineSharp.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class SpawnLivingEntityPacket : Packet {

        public int EntityId { get; private set; }
        public UUID EntityUUID { get; private set; }
        public int Type { get; private set; }
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }
        public Angle? Pitch { get; private set; }
        public Angle? Yaw { get; private set; }
        public Angle? HeadPitch { get; private set; }
        public short VelocityX { get; private set; }
        public short VelocityY { get; private set; }
        public short VelocityZ { get; private set; }

        public override void Read(PacketBuffer buffer) {
            this.EntityId = buffer.ReadVarInt();
            this.EntityUUID = buffer.ReadUUID();
            this.Type = buffer.ReadVarInt();
            this.X = buffer.ReadDouble();
            this.Y = buffer.ReadDouble();
            this.Z = buffer.ReadDouble();
            this.Pitch = Angle.FromByte(buffer.ReadByte());
            this.Yaw = Angle.FromByte(buffer.ReadByte());
            this.HeadPitch = Angle.FromByte(buffer.ReadByte());
            this.VelocityX = buffer.ReadShort();
            this.VelocityY = buffer.ReadShort();
            this.VelocityZ = buffer.ReadShort();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.EntityId);
            buffer.WriteUUID(this.EntityUUID);
            buffer.WriteVarInt((int)this.Type);
            buffer.WriteDouble(this.X);
            buffer.WriteDouble(this.Y);
            buffer.WriteDouble(this.Z);
            buffer.WriteByte(this.Pitch.ToByte());
            buffer.WriteByte(this.Yaw.ToByte());
            buffer.WriteByte(this.HeadPitch.ToByte());
            buffer.WriteShort(this.VelocityX);
            buffer.WriteShort(this.VelocityY);
            buffer.WriteShort(this.VelocityZ);
        }

        public override async Task Handle(MinecraftClient client) {
            await base.Handle(client);
            //Logger.Debug("New Entity spawned: " + Enum.GetName(typeof(EntityId), (EntityId)this.Type) + $" at ({X.ToString().Replace(",", ".")}, {Y.ToString().Replace(",", ".")}, {Z.ToString().Replace(",", ".")})");
        }
    }
}
