namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class MapDataPacket : Packet {

        public int MapID { get; private set; }
public byte Scale { get; private set; }
public bool Locked { get; private set; }
public bool TrackingPosition { get; private set; }
public int IconCount { get; private set; }
public byte X { get; private set; }
public byte Z { get; private set; }
public byte Direction { get; private set; }
public bool HasDisplayName { get; private set; }
public byte Columns { get; private set; }

        public MapDataPacket() { }

        public MapDataPacket(int mapid, byte scale, bool locked, bool trackingposition, int iconcount, byte x, byte z, byte direction, bool hasdisplayname, byte columns) {
            this.MapID = mapid;
this.Scale = scale;
this.Locked = locked;
this.TrackingPosition = trackingposition;
this.IconCount = iconcount;
this.X = x;
this.Z = z;
this.Direction = direction;
this.HasDisplayName = hasdisplayname;
this.Columns = columns;
        }

        public override void Read(PacketBuffer buffer) {
            this.MapID = buffer.ReadVarInt();
this.Scale = buffer.ReadByte();
this.Locked = buffer.ReadBoolean();
this.TrackingPosition = buffer.ReadBoolean();
this.IconCount = buffer.ReadVarInt();
this.X = buffer.ReadByte();
this.Z = buffer.ReadByte();
this.Direction = buffer.ReadByte();
this.HasDisplayName = buffer.ReadBoolean();
this.Columns = buffer.ReadByte();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt(this.MapID);
buffer.WriteByte(this.Scale);
buffer.WriteBoolean(this.Locked);
buffer.WriteBoolean(this.TrackingPosition);
buffer.WriteVarInt(this.IconCount);
buffer.WriteByte(this.X);
buffer.WriteByte(this.Z);
buffer.WriteByte(this.Direction);
buffer.WriteBoolean(this.HasDisplayName);
buffer.WriteByte(this.Columns);
        }
    }
}