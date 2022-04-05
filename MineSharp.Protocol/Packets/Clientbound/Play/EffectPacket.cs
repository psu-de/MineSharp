using MineSharp.Core.Types;
namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class EffectPacket : Packet {

        public int EffectID { get; private set; }
        public Position? Location { get; private set; }
        public int Data { get; private set; }
        public bool DisableRelativeVolume { get; private set; }

        public EffectPacket() { }

        public EffectPacket(int effectid, Position? location, int data, bool disablerelativevolume) {
            this.EffectID = effectid;
            this.Location = location;
            this.Data = data;
            this.DisableRelativeVolume = disablerelativevolume;
        }

        public override void Read(PacketBuffer buffer) {
            this.EffectID = buffer.ReadInt();
            this.Location = buffer.ReadPosition();
            this.Data = buffer.ReadInt();
            this.DisableRelativeVolume = buffer.ReadBoolean();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteInt(this.EffectID);
            buffer.WritePosition(this.Location);
            buffer.WriteInt(this.Data);
            buffer.WriteBoolean(this.DisableRelativeVolume);
        }
    }
}