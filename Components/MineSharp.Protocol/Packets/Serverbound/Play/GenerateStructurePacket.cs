using MineSharp.Core.Types;

namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class GenerateStructurePacket : Packet {

        public Position? Location { get; private set; }
        public int Levels { get; private set; }
        public bool KeepJigsaws { get; private set; }

        public GenerateStructurePacket() { }

        public GenerateStructurePacket(Position location, int levels, bool keepjigsaws) {
            this.Location = location;
            this.Levels = levels;
            this.KeepJigsaws = keepjigsaws;
        }

        public override void Read(PacketBuffer buffer) {
            this.Location = buffer.ReadPosition();
            this.Levels = buffer.ReadVarInt();
            this.KeepJigsaws = buffer.ReadBoolean();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WritePosition(this.Location!);
            buffer.WriteVarInt(this.Levels);
            buffer.WriteBoolean(this.KeepJigsaws);
        }
    }
}