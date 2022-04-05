namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class EntityPositionPacket : Packet {

        public int EntityID { get; private set; }
public short DeltaX { get; private set; }
public short DeltaY { get; private set; }
public short DeltaZ { get; private set; }
public bool OnGround { get; private set; }

        public EntityPositionPacket() { }

        public EntityPositionPacket(int entityid, short deltax, short deltay, short deltaz, bool onground) {
            this.EntityID = entityid;
this.DeltaX = deltax;
this.DeltaY = deltay;
this.DeltaZ = deltaz;
this.OnGround = onground;
        }

        public override void Read(PacketBuffer buffer) {
            this.EntityID = buffer.ReadVarInt();
this.DeltaX = buffer.ReadShort();
this.DeltaY = buffer.ReadShort();
this.DeltaZ = buffer.ReadShort();
this.OnGround = buffer.ReadBoolean();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.EntityID);
buffer.WriteShort(this.DeltaX);
buffer.WriteShort(this.DeltaY);
buffer.WriteShort(this.DeltaZ);
buffer.WriteBoolean(this.OnGround);
        }
    }
}