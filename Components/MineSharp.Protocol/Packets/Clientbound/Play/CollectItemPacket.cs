namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class CollectItemPacket : Packet {

        public int CollectedEntityID { get; private set; }
public int CollectorEntityID { get; private set; }
public int PickupItemCount { get; private set; }

        public CollectItemPacket() { }

        public CollectItemPacket(int collectedentityid, int collectorentityid, int pickupitemcount) {
            this.CollectedEntityID = collectedentityid;
this.CollectorEntityID = collectorentityid;
this.PickupItemCount = pickupitemcount;
        }

        public override void Read(PacketBuffer buffer) {
            this.CollectedEntityID = buffer.ReadVarInt();
this.CollectorEntityID = buffer.ReadVarInt();
this.PickupItemCount = buffer.ReadVarInt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.CollectedEntityID);
buffer.WriteVarInt(this.CollectorEntityID);
buffer.WriteVarInt(this.PickupItemCount);
        }
    }
}