using MineSharp.Core.Types;

namespace MineSharp.Protocol.Packets.Serverbound.Play {
    public class SetDisplayedRecipePacket : Packet {

        public Identifier? RecipeID { get; private set; }

        public SetDisplayedRecipePacket() { }

        public SetDisplayedRecipePacket(Identifier? recipeid) {
            this.RecipeID = recipeid;
        }

        public override void Read(PacketBuffer buffer) {
            this.RecipeID = buffer.ReadIdentifier();
        }

        public override void Write(PacketBuffer buffer) {
            buffer.WriteIdentifier(this.RecipeID!);
        }
    }
}