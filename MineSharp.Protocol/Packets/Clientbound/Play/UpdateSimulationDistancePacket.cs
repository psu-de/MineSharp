namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class UpdateSimulationDistancePacket : Packet {

        public int SimulationDistance { get; private set; }

        public UpdateSimulationDistancePacket() { }

        public UpdateSimulationDistancePacket(int simulationdistance) {
            this.SimulationDistance = simulationdistance;
        }

        public override void Read(PacketBuffer buffer) {
            this.SimulationDistance = buffer.ReadVarInt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.SimulationDistance);
        }
    }
}