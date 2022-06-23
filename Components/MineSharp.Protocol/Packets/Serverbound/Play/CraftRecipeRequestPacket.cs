using MineSharp.Core.Types;

namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class CraftRecipeRequestPacket : Packet {

        public byte WindowID { get; private set; }
        public Identifier? Recipe { get; private set; }
        public bool Makeall { get; private set; }

        public CraftRecipeRequestPacket() { }

        public CraftRecipeRequestPacket(byte windowid, Identifier recipe, bool makeall) {
            this.WindowID = windowid;
            this.Recipe = recipe;
            this.Makeall = makeall;
        }

        public override void Read(PacketBuffer buffer) {
            this.WindowID = buffer.ReadByte();
            this.Recipe = buffer.ReadIdentifier();
            this.Makeall = buffer.ReadBoolean();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteByte(this.WindowID);
            buffer.WriteIdentifier(this.Recipe!);
            buffer.WriteBoolean(this.Makeall);
        }
    }
}