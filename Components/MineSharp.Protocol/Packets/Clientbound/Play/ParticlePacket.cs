namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class ParticlePacket : Packet {

        public int ParticleID { get; private set; }
        public bool LongDistance { get; private set; }
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }
        public float OffsetX { get; private set; }
        public float OffsetY { get; private set; }
        public float OffsetZ { get; private set; }
        public float ParticleData { get; private set; }
        public int ParticleCount { get; private set; }
        public object? Data { get; private set; }

        public ParticlePacket() { }

        public ParticlePacket(int particleid, bool longdistance, double x, double y, double z, float offsetx, float offsety, float offsetz, float particledata, int particlecount, object? data) {
            this.ParticleID = particleid;
            this.LongDistance = longdistance;
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.OffsetX = offsetx;
            this.OffsetY = offsety;
            this.OffsetZ = offsetz;
            this.ParticleData = particledata;
            this.ParticleCount = particlecount;
            this.Data = data;
        }

        public override void Read(PacketBuffer buffer) {
            this.ParticleID = buffer.ReadInt();
            this.LongDistance = buffer.ReadBoolean();
            this.X = buffer.ReadDouble();
            this.Y = buffer.ReadDouble();
            this.Z = buffer.ReadDouble();
            this.OffsetX = buffer.ReadFloat();
            this.OffsetY = buffer.ReadFloat();
            this.OffsetZ = buffer.ReadFloat();
            this.ParticleData = buffer.ReadFloat();
            this.ParticleCount = buffer.ReadInt();

            float fromRed;
            float fromGreen;
            float fromBlue;
            float scale;

            switch (this.ParticleID) {
                case 2:
                case 3:
                case 24:
                    this.Data = new { BlockState = buffer.ReadVarInt() };
                    break;
                case 14:
                    fromRed = buffer.ReadFloat();
                    fromGreen = buffer.ReadFloat();
                    fromBlue = buffer.ReadFloat();
                    scale = buffer.ReadFloat();
                    this.Data = new { FromRed = fromRed, FromGreen = fromGreen, FromBlue = fromBlue, Scale = scale };
                    break;
                case 15:
                    fromRed = buffer.ReadFloat();
                    fromGreen = buffer.ReadFloat();
                    fromBlue = buffer.ReadFloat();
                    scale = buffer.ReadFloat();
                    float toRed = buffer.ReadFloat();
                    float toGreen = buffer.ReadFloat();
                    float toBlue = buffer.ReadFloat();
                    this.Data = new { FromRed = fromRed, FromGreen = fromGreen, FromBlue = fromBlue, Scale = scale, ToRed = toRed, ToGreen = toGreen, ToBlue = toBlue };
                    break;
                case 35:
                    this.Data = new { Item = buffer.ReadSlot() };
                    break;
                case 36:
                    this.Data = new { Origin = buffer.ReadPosition(), PositionType = buffer.ReadString(), BlockPosition = buffer.ReadPosition(), EntityId = buffer.ReadVarInt(), Ticks = buffer.ReadVarInt() };
                    break;
            }
        }

        public override void Write(PacketBuffer buffer) {
            //buffer.WriteInt(this.ParticleID);
            //buffer.WriteBoolean(this.LongDistance);
            //buffer.WriteDouble(this.X);
            //buffer.WriteDouble(this.Y);
            //buffer.WriteDouble(this.Z);
            //buffer.WriteFloat(this.OffsetX);
            //buffer.WriteFloat(this.OffsetY);
            //buffer.WriteFloat(this.OffsetZ);
            //buffer.WriteFloat(this.ParticleData);
            //buffer.WriteInt(this.ParticleCount);
            throw new NotSupportedException();
        }        
    }
}