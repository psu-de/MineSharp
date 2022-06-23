namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class SetPassengersPacket : Packet {

        public int EntityID { get; private set; }
        public int PassengerCount { get; private set; }
        public int[]? Passengers { get; private set; }

        public SetPassengersPacket() { }

        public SetPassengersPacket(int entityid, int[] passengers) {
            this.EntityID = entityid;
            this.Passengers = passengers;
        }

        public override void Read(PacketBuffer buffer) {
            this.EntityID = buffer.ReadVarInt();
            this.Passengers = buffer.ReadVarIntArray();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.EntityID);
            buffer.WriteVarIntArray(this.Passengers!);
        }
    }
}