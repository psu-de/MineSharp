using MineSharp.Core.Types;

namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class CraftRecipeResponsePacket : Packet {

        public byte WindowID { get; private set; }
        public Identifier? Recipe { get; private set; }

        public CraftRecipeResponsePacket() { }

        public CraftRecipeResponsePacket(byte windowid, Identifier? recipe) {
            this.WindowID = windowid;
            this.Recipe = recipe;
        }

        public override void Read(PacketBuffer buffer) {
            this.WindowID = buffer.ReadByte();
            this.Recipe = buffer.ReadIdentifier();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteByte(this.WindowID);
            buffer.WriteIdentifier(this.Recipe);
        }
    }
}