using MineSharp.Core.Types;
namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class SculkVibrationSignalPacket : Packet {

        public Position? SourcePosition { get; private set; }
        public Identifier? DestinationIdentifier { get; private set; }
        public int ArrivalTicks { get; private set; }

        public SculkVibrationSignalPacket() { }

        public SculkVibrationSignalPacket(Position sourceposition, Identifier destinationidentifier, int arrivalticks) {
            this.SourcePosition = sourceposition;
            this.DestinationIdentifier = destinationidentifier;
            this.ArrivalTicks = arrivalticks;
        }

        public override void Read(PacketBuffer buffer) {
            this.SourcePosition = buffer.ReadPosition();
            this.DestinationIdentifier = buffer.ReadIdentifier();
            this.ArrivalTicks = buffer.ReadVarInt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WritePosition(this.SourcePosition!);
            buffer.WriteIdentifier(this.DestinationIdentifier!);
            buffer.WriteVarInt(this.ArrivalTicks);
        }
    }
}