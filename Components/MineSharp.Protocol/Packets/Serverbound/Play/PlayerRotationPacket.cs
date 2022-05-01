namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class PlayerRotationPacket : Packet {

        public float Yaw { get; private set; }
public float Pitch { get; private set; }
public bool OnGround { get; private set; }

        public PlayerRotationPacket() { }

        public PlayerRotationPacket(float yaw, float pitch, bool onground) {
            this.Yaw = yaw;
this.Pitch = pitch;
this.OnGround = onground;
        }

        public override void Read(PacketBuffer buffer) {
            this.Yaw = buffer.ReadFloat();
this.Pitch = buffer.ReadFloat();
this.OnGround = buffer.ReadBoolean();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteFloat(this.Yaw);
buffer.WriteFloat(this.Pitch);
buffer.WriteBoolean(this.OnGround);
        }
    }
}