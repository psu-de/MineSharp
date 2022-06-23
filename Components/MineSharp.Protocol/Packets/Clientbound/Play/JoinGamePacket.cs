using fNbt;
using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;

namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class JoinGamePacket : Packet {

        public int EntityID { get; private set; }
        public bool Ishardcore { get; private set; }
        public GameMode Gamemode { get; private set; }
        public GameMode PreviousGamemode { get; private set; }
        public Identifier[]? DimensionNames { get; private set; }
        public NbtCompound? DimensionCodec { get; private set; }
        public NbtCompound? Dimension { get; private set; }
        public Identifier? DimensionName { get; private set; }
        public long Hashedseed { get; private set; }
        public int MaxPlayers { get; private set; }
        public int ViewDistance { get; private set; }
        public int SimulationDistance { get; private set; }
        public bool ReducedDebugInfo { get; private set; }
        public bool Enablerespawnscreen { get; private set; }
        public bool IsDebug { get; private set; }
        public bool IsFlat { get; private set; }

        public JoinGamePacket() { }

        public JoinGamePacket(int entityid, bool ishardcore, GameMode gamemode, GameMode previousgamemode, Identifier[] dimensionnames, NbtCompound? dimensioncodec, NbtCompound? dimension, Identifier dimensionname, long hashedseed, int maxplayers, int viewdistance, int simulationdistance, bool reduceddebuginfo, bool enablerespawnscreen, bool isdebug, bool isflat) {
            this.EntityID = entityid;
            this.Ishardcore = ishardcore;
            this.Gamemode = gamemode;
            this.PreviousGamemode = previousgamemode;
            this.DimensionNames = dimensionnames;
            this.DimensionCodec = dimensioncodec;
            this.Dimension = dimension;
            this.DimensionName = dimensionname;
            this.Hashedseed = hashedseed;
            this.MaxPlayers = maxplayers;
            this.ViewDistance = viewdistance;
            this.SimulationDistance = simulationdistance;
            this.ReducedDebugInfo = reduceddebuginfo;
            this.Enablerespawnscreen = enablerespawnscreen;
            this.IsDebug = isdebug;
            this.IsFlat = isflat;
        }

        public override void Read(PacketBuffer buffer) {
            this.EntityID = buffer.ReadInt();
            this.Ishardcore = buffer.ReadBoolean();
            this.Gamemode = (GameMode)buffer.ReadByte();
            this.PreviousGamemode = (GameMode)buffer.ReadByte();
            this.DimensionNames = buffer.ReadIdentifierArray();
            this.DimensionCodec = buffer.ReadNBTCompound();
            this.Dimension = buffer.ReadNBTCompound();
            this.DimensionName = buffer.ReadIdentifier();
            this.Hashedseed = buffer.ReadLong();
            this.MaxPlayers = buffer.ReadVarInt();
            this.ViewDistance = buffer.ReadVarInt();
            this.SimulationDistance = buffer.ReadVarInt();
            this.ReducedDebugInfo = buffer.ReadBoolean();
            this.Enablerespawnscreen = buffer.ReadBoolean();
            this.IsDebug = buffer.ReadBoolean();
            this.IsFlat = buffer.ReadBoolean();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteInt(this.EntityID);
            buffer.WriteBoolean(this.Ishardcore);
            buffer.WriteByte((byte)this.Gamemode);
            buffer.WriteByte((byte)this.PreviousGamemode);
            buffer.WriteIdentifierArray(this.DimensionNames!);
            buffer.WriteNBTCompound(this.DimensionCodec);
            buffer.WriteNBTCompound(this.Dimension);
            buffer.WriteIdentifier(this.DimensionName!);
            buffer.WriteLong(this.Hashedseed);
            buffer.WriteVarInt(this.MaxPlayers);
            buffer.WriteVarInt(this.ViewDistance);
            buffer.WriteVarInt(this.SimulationDistance);
            buffer.WriteBoolean(this.ReducedDebugInfo);
            buffer.WriteBoolean(this.Enablerespawnscreen);
            buffer.WriteBoolean(this.IsDebug);
            buffer.WriteBoolean(this.IsFlat);
        }
    }
}