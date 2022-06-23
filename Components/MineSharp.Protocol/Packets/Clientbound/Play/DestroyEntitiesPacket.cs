namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class DestroyEntitiesPacket : Packet {

        public int[]? EntityIds { get; private set; }

        public DestroyEntitiesPacket() { }
        public DestroyEntitiesPacket(int[] EntityIds) {
            this.EntityIds = EntityIds;
        }

        public override void Read(PacketBuffer buffer) {
            int len = buffer.ReadVarInt();
            this.EntityIds = new int[len];
            for (int i = 0; i < len; i++) this.EntityIds[i] = buffer.ReadVarInt();
        }

        public override void Write(PacketBuffer buffer) {
            throw new NotImplementedException();
        }
    }
}
