using MineSharp.Core.Types;

namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class UpdateJigsawBlockPacket : Packet {

        public Position? Location { get; private set; }
        public Identifier? Name { get; private set; }
        public Identifier? Target { get; private set; }
        public Identifier? Pool { get; private set; }
        public string Finalstate { get; private set; }
        public string Jointtype { get; private set; }

        public UpdateJigsawBlockPacket() { }

        public UpdateJigsawBlockPacket(Position? location, Identifier? name, Identifier? target, Identifier? pool, string finalstate, string jointtype) {
            this.Location = location;
            this.Name = name;
            this.Target = target;
            this.Pool = pool;
            this.Finalstate = finalstate;
            this.Finalstate = finalstate;
            this.Jointtype = jointtype;
        }

        public override void Read(PacketBuffer buffer) {
            this.Location = buffer.ReadPosition();
            this.Name = buffer.ReadIdentifier();
            this.Target = buffer.ReadIdentifier();
            this.Pool = buffer.ReadIdentifier();
            this.Finalstate = buffer.ReadString();
            this.Jointtype = buffer.ReadString();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WritePosition(this.Location);
            buffer.WriteIdentifier(this.Name);
            buffer.WriteIdentifier(this.Target);
            buffer.WriteIdentifier(this.Pool);
            buffer.WriteString(this.Finalstate);
            buffer.WriteString(this.Jointtype);
        }
    }
}