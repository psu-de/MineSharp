namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class UpdateHealthPacket : Packet {

        public float Health { get; private set; }
public int Food { get; private set; }
public float FoodSaturation { get; private set; }

        public UpdateHealthPacket() { }

        public UpdateHealthPacket(float health, int food, float foodsaturation) {
            this.Health = health;
this.Food = food;
this.FoodSaturation = foodsaturation;
        }

        public override void Read(PacketBuffer buffer) {
            this.Health = buffer.ReadFloat();
this.Food = buffer.ReadVarInt();
this.FoodSaturation = buffer.ReadFloat();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteFloat(this.Health);
buffer.WriteVarInt(this.Food);
buffer.WriteFloat(this.FoodSaturation);
        }
    }
}