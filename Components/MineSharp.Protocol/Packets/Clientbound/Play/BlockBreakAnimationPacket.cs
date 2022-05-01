using MineSharp.Core.Types;

namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class BlockBreakAnimationPacket : Packet {

        public int EntityID { get; private set; }
        public Position? Location { get; private set; }
        public byte DestroyStage { get; private set; }

        public BlockBreakAnimationPacket() { }

        public BlockBreakAnimationPacket(int entityid, Position? location, byte destroystage) {
            this.EntityID = entityid;
            this.Location = location;
            this.DestroyStage = destroystage;
        }

        public override void Read(PacketBuffer buffer) {
            this.EntityID = buffer.ReadVarInt();
            this.Location = buffer.ReadPosition();
            this.DestroyStage = buffer.ReadByte();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.EntityID);
            buffer.WritePosition(this.Location);
            buffer.WriteByte(this.DestroyStage);
        }
    }
}