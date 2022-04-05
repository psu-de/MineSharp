namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class PlayerAbilitiesPacket : Packet {

        public byte Flags { get; private set; }
    public float FlyingSpeed { get; private set; }
    public float FieldofViewModifier { get; private set; }

    public PlayerAbilitiesPacket() { }

    public PlayerAbilitiesPacket(byte flags, float flyingspeed, float fieldofviewmodifier) {
        this.Flags = flags;
        this.FlyingSpeed = flyingspeed;
        this.FieldofViewModifier = fieldofviewmodifier;
    }

    public override void Read(PacketBuffer buffer) {
        this.Flags = buffer.ReadByte();
        this.FlyingSpeed = buffer.ReadFloat();
        this.FieldofViewModifier = buffer.ReadFloat();
    }

    public override void Write(PacketBuffer buffer) {
        buffer.WriteByte(this.Flags);
        buffer.WriteFloat(this.FlyingSpeed);
        buffer.WriteFloat(this.FieldofViewModifier);
    }
}
}