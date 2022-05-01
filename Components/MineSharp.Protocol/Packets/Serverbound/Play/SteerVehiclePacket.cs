namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class SteerVehiclePacket : Packet {

        public float Sideways { get; private set; }
public float Forward { get; private set; }
public byte Flags { get; private set; }

        public SteerVehiclePacket() { }

        public SteerVehiclePacket(float sideways, float forward, byte flags) {
            this.Sideways = sideways;
this.Forward = forward;
this.Flags = flags;
        }

        public override void Read(PacketBuffer buffer) {
            this.Sideways = buffer.ReadFloat();
this.Forward = buffer.ReadFloat();
this.Flags = buffer.ReadByte();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteFloat(this.Sideways);
buffer.WriteFloat(this.Forward);
buffer.WriteByte(this.Flags);
        }
    }
}