namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class InteractEntityPacket : Packet {

        public int EntityID { get; private set; }
        public InteractMode Type { get; private set; }
        public bool Sneaking { get; private set; }

        public InteractEntityPacket() { }

        public InteractEntityPacket(int entityid, InteractMode type, bool sneaking) {
            this.EntityID = entityid;
            this.Type = type;
            this.Sneaking = sneaking;
        }

        public override void Read(PacketBuffer buffer) {
            this.EntityID = buffer.ReadVarInt();
            this.Type = (InteractMode)buffer.ReadVarInt();
            this.Sneaking = buffer.ReadBoolean();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.EntityID);
            buffer.WriteVarInt((int)this.Type);
            buffer.WriteBoolean(this.Sneaking);
        }

        public enum InteractMode {
            Interact = 0,
            Attack = 1,
            InteractAt = 2
        }
    }
}