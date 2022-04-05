using fNbt;
using MineSharp.Core.Types;
namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class RespawnPacket : Packet {

        public NbtCompound? Dimension { get; private set; }
        public Identifier? DimensionName { get; private set; }
        public long Hashedseed { get; private set; }
        public byte Gamemode { get; private set; }
        public byte PreviousGamemode { get; private set; }
        public bool IsDebug { get; private set; }
        public bool IsFlat { get; private set; }
        public bool Copymetadata { get; private set; }

        public RespawnPacket() { }

        public RespawnPacket(NbtCompound? dimension, Identifier? dimensionname, long hashedseed, byte gamemode, byte previousgamemode, bool isdebug, bool isflat, bool copymetadata) {
            this.Dimension = dimension;
            this.DimensionName = dimensionname;
            this.Hashedseed = hashedseed;
            this.Gamemode = gamemode;
            this.PreviousGamemode = previousgamemode;
            this.IsDebug = isdebug;
            this.IsFlat = isflat;
            this.Copymetadata = copymetadata;
        }

        public override void Read(PacketBuffer buffer) {
            this.Dimension = buffer.ReadNBTCompound();
            this.DimensionName = buffer.ReadIdentifier();
            this.Hashedseed = buffer.ReadLong();
            this.Gamemode = buffer.ReadByte();
            this.PreviousGamemode = buffer.ReadByte();
            this.IsDebug = buffer.ReadBoolean();
            this.IsFlat = buffer.ReadBoolean();
            this.Copymetadata = buffer.ReadBoolean();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteNBTCompound(this.Dimension);
            buffer.WriteIdentifier(this.DimensionName);
            buffer.WriteLong(this.Hashedseed);
            buffer.WriteByte(this.Gamemode);
            buffer.WriteByte(this.PreviousGamemode);
            buffer.WriteBoolean(this.IsDebug);
            buffer.WriteBoolean(this.IsFlat);
            buffer.WriteBoolean(this.Copymetadata);
        }
    }
}