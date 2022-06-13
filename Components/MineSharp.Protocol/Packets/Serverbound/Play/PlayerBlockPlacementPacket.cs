using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;

namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class PlayerBlockPlacementPacket : Packet {

        public PlayerHand Hand { get; private set; }
        public Position? Location { get; private set; }
        public BlockFace Face { get; private set; }
        public float CursorPositionX { get; private set; }
        public float CursorPositionY { get; private set; }
        public float CursorPositionZ { get; private set; }
        public bool Insideblock { get; private set; }

        public PlayerBlockPlacementPacket() { }

        public PlayerBlockPlacementPacket(PlayerHand hand, Position? location, BlockFace face, float cursorpositionx, float cursorpositiony, float cursorpositionz, bool insideblock) {
            this.Hand = hand;
            this.Location = location;
            this.Face = face;
            this.CursorPositionX = cursorpositionx;
            this.CursorPositionY = cursorpositiony;
            this.CursorPositionZ = cursorpositionz;
            this.Insideblock = insideblock;
        }

        public override void Read(PacketBuffer buffer) {
            this.Hand = (PlayerHand)buffer.ReadVarInt();
            this.Location = buffer.ReadPosition();
            this.Face = (BlockFace)buffer.ReadVarInt();
            this.CursorPositionX = buffer.ReadFloat();
            this.CursorPositionY = buffer.ReadFloat();
            this.CursorPositionZ = buffer.ReadFloat();
            this.Insideblock = buffer.ReadBoolean();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteVarInt((int)this.Hand);
            buffer.WritePosition(this.Location);
            buffer.WriteVarInt((int)this.Face);
            buffer.WriteFloat(this.CursorPositionX);
            buffer.WriteFloat(this.CursorPositionY);
            buffer.WriteFloat(this.CursorPositionZ);
            buffer.WriteBoolean(this.Insideblock);
        }
    }
}