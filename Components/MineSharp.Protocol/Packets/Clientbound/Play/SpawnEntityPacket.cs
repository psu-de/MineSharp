﻿using MineSharp.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class SpawnEntityPacket : Packet {

        public int EntityId { get; private set; }
        public UUID UUID { get; private set; }
        public int Type { get; private set; }   
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }
        public Angle? Pitch { get; private set; }
        public Angle? Yaw { get; private set; }
        public int Data { get; private set; }
        public short VelocityX { get; private set; }
        public short VelocityY { get; private set; }
        public short VelocityZ { get; private set; }

        public SpawnEntityPacket() { }

        public SpawnEntityPacket(int entityId, UUID uuid, int type, double x, double y, double z, Angle pitch, Angle yaw, int data, short velX, short velY, short velZ) {
            EntityId = entityId;
            UUID = uuid;
            Type = type;
            X = x;
            Y = y;
            Z = z;
            Pitch = pitch;
            Yaw = yaw;
            Data = data;
            VelocityX = velX;
            VelocityY = velY;
            VelocityZ = velZ;
        }

        public override void Read(PacketBuffer buffer) {
            this.EntityId = buffer.ReadVarInt();
            this.UUID = buffer.ReadUUID();
            this.Type = buffer.ReadVarInt();
            this.X = buffer.ReadDouble();
            this.Y = buffer.ReadDouble();
            this.Z = buffer.ReadDouble();
            this.Pitch = buffer.ReadAngle();
            this.Yaw = buffer.ReadAngle();
            this.Data = buffer.ReadInt();
            this.VelocityX = buffer.ReadShort();
            this.VelocityY = buffer.ReadShort();
            this.VelocityZ = buffer.ReadShort();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.EntityId);
            buffer.WriteUUID(this.UUID);
            buffer.WriteVarInt(this.Type);
            buffer.WriteDouble(this.X);
            buffer.WriteDouble(this.Y);
            buffer.WriteDouble(this.Z);
            buffer.WriteByte(this.Pitch.ToByte());
            buffer.WriteByte(this.Yaw.ToByte());
            buffer.WriteInt(this.Data);
            buffer.WriteShort(this.VelocityX);
            buffer.WriteShort(this.VelocityY);
            buffer.WriteShort(this.VelocityZ);
        }
    }
}