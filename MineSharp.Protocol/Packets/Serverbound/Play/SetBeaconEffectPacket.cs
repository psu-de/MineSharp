namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class SetBeaconEffectPacket : Packet {

        public int PrimaryEffect { get; private set; }
public int SecondaryEffect { get; private set; }

        public SetBeaconEffectPacket() { }

        public SetBeaconEffectPacket(int primaryeffect, int secondaryeffect) {
            this.PrimaryEffect = primaryeffect;
this.SecondaryEffect = secondaryeffect;
        }

        public override void Read(PacketBuffer buffer) {
            this.PrimaryEffect = buffer.ReadVarInt();
this.SecondaryEffect = buffer.ReadVarInt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.PrimaryEffect);
buffer.WriteVarInt(this.SecondaryEffect);
        }
    }
}