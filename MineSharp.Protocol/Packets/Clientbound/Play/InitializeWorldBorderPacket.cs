namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class InitializeWorldBorderPacket : Packet {

        public double X { get; private set; }
public double Z { get; private set; }
public double OldDiameter { get; private set; }
public double NewDiameter { get; private set; }
public long Speed { get; private set; }
public int PortalTeleportBoundary { get; private set; }
public int WarningBlocks { get; private set; }
public int WarningTime { get; private set; }

        public InitializeWorldBorderPacket() { }

        public InitializeWorldBorderPacket(double x, double z, double olddiameter, double newdiameter, long speed, int portalteleportboundary, int warningblocks, int warningtime) {
            this.X = x;
this.Z = z;
this.OldDiameter = olddiameter;
this.NewDiameter = newdiameter;
this.Speed = speed;
this.PortalTeleportBoundary = portalteleportboundary;
this.WarningBlocks = warningblocks;
this.WarningTime = warningtime;
        }

        public override void Read(PacketBuffer buffer) {
            this.X = buffer.ReadDouble();
this.Z = buffer.ReadDouble();
this.OldDiameter = buffer.ReadDouble();
this.NewDiameter = buffer.ReadDouble();
this.Speed = buffer.ReadVarLong();
this.PortalTeleportBoundary = buffer.ReadVarInt();
this.WarningBlocks = buffer.ReadVarInt();
this.WarningTime = buffer.ReadVarInt();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteDouble(this.X);
buffer.WriteDouble(this.Z);
buffer.WriteDouble(this.OldDiameter);
buffer.WriteDouble(this.NewDiameter);
buffer.WriteVarLong(this.Speed);
buffer.WriteVarInt(this.PortalTeleportBoundary);
buffer.WriteVarInt(this.WarningBlocks);
buffer.WriteVarInt(this.WarningTime);
        }
    }
}