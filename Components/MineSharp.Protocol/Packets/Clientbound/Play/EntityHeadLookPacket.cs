using MineSharp.Core.Types;
namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class EntityHeadLookPacket : Packet {

        public int EntityID { get; private set; }
        public Angle? HeadYaw { get; private set; }

        public EntityHeadLookPacket() { }

        public EntityHeadLookPacket(int entityid, Angle headyaw) {
            this.EntityID = entityid;
            this.HeadYaw = headyaw;
        }

        public override void Read(PacketBuffer buffer) {
            this.EntityID = buffer.ReadVarInt();
            this.HeadYaw = buffer.ReadAngle();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.EntityID);
            buffer.WriteAngle(this.HeadYaw!);
        }
    }
}