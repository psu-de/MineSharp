namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class ExplosionPacket : Packet {

        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }
        public float Strength { get; private set; }
        public sbyte[] Records { get; private set; }
        public float PlayerMotionX { get; private set; }
        public float PlayerMotionY { get; private set; }
        public float PlayerMotionZ { get; private set; }

        public ExplosionPacket() { }

        public ExplosionPacket(float x, float y, float z, float strength, sbyte[] records, float playermotionx, float playermotiony, float playermotionz) {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Strength = strength;
            this.Records = records;
            this.PlayerMotionX = playermotionx;
            this.PlayerMotionY = playermotiony;
            this.PlayerMotionZ = playermotionz;
        }

        public override void Read(PacketBuffer buffer) {
            this.X = buffer.ReadFloat();
            this.Y = buffer.ReadFloat();
            this.Z = buffer.ReadFloat();
            this.Strength = buffer.ReadFloat();
            
            int length = buffer.ReadVarInt();
            this.Records = new sbyte[length];
            for (int i = 0; i < length; i++) this.Records[i] = (sbyte)buffer.ReadByte();

            this.PlayerMotionX = buffer.ReadFloat();
            this.PlayerMotionY = buffer.ReadFloat();
            this.PlayerMotionZ = buffer.ReadFloat();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteFloat(this.X);
            buffer.WriteFloat(this.Y);
            buffer.WriteFloat(this.Z);
            buffer.WriteFloat(this.Strength);
            buffer.WriteByteArray(this.Records.Cast<byte>().ToArray());
            buffer.WriteFloat(this.PlayerMotionX);
            buffer.WriteFloat(this.PlayerMotionY);
            buffer.WriteFloat(this.PlayerMotionZ);
        }
    }
}