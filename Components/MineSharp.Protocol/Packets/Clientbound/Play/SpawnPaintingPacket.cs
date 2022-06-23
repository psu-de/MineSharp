using MineSharp.Core.Types;
namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class SpawnPaintingPacket : Packet {

        public int EntityID { get; private set; }
        public UUID? EntityUUID { get; private set; }
        public int Motive { get; private set; }
        public Position? Location { get; private set; }
        public byte Direction { get; private set; }

        public SpawnPaintingPacket() { }

        public SpawnPaintingPacket(int entityid, UUID entityuuid, int motive, Position location, byte direction) {
            this.EntityID = entityid;
            this.EntityUUID = entityuuid;
            this.Motive = motive;
            this.Location = location;
            this.Direction = direction;
        }

        public override void Read(PacketBuffer buffer) {
            this.EntityID = buffer.ReadVarInt();
            this.EntityUUID = buffer.ReadUUID();
            this.Motive = buffer.ReadVarInt();
            this.Location = buffer.ReadPosition();
            this.Direction = buffer.ReadByte();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.EntityID);
            buffer.WriteUUID((UUID)this.EntityUUID!);
            buffer.WriteVarInt(this.Motive);
            buffer.WritePosition(this.Location!);
            buffer.WriteByte(this.Direction);
        }
    }
}