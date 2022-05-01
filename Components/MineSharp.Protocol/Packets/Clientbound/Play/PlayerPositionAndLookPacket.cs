namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class PlayerPositionAndLookPacket : Packet {

        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }
        public float Yaw { get; private set; }
        public float Pitch { get; private set; }
        public byte Flags { get; private set; }
        public int TeleportID { get; private set; }
        public bool DismountVehicle { get; private set; }

        public PlayerPositionAndLookPacket() { }

        public PlayerPositionAndLookPacket(double x, double y, double z, float yaw, float pitch, byte flags, int teleportid, bool dismountvehicle) {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Yaw = yaw;
            this.Pitch = pitch;
            this.Flags = flags;
            this.TeleportID = teleportid;
            this.DismountVehicle = dismountvehicle;
        }

        public override void Read(PacketBuffer buffer) {
            this.X = buffer.ReadDouble();
            this.Y = buffer.ReadDouble();
            this.Z = buffer.ReadDouble();
            this.Yaw = buffer.ReadFloat();
            this.Pitch = buffer.ReadFloat();
            this.Flags = buffer.ReadByte();
            this.TeleportID = buffer.ReadVarInt();
            this.DismountVehicle = buffer.ReadBoolean();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteDouble(this.X);
            buffer.WriteDouble(this.Y);
            buffer.WriteDouble(this.Z);
            buffer.WriteFloat(this.Yaw);
            buffer.WriteFloat(this.Pitch);
            buffer.WriteByte(this.Flags);
            buffer.WriteVarInt(this.TeleportID);
            buffer.WriteBoolean(this.DismountVehicle);
        }
    }
}