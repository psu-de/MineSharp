using MineSharp.Protocol.Packets;

namespace MineSharp.World.PalettedContainer.Palettes {
    internal class SingleValuePalette : IPalette {

        public int Value;

        public int Get(int entry) {
            return Value;
        }

        public bool HasState(int minState, int maxState) {
            return minState <= Value && Value <= maxState;
        }

        public void Read(PacketBuffer buffer) {
            this.Value = buffer.ReadVarInt();
        }

        public IndirectPalette ConvertToIndirectPalette(int newState) {
            return new IndirectPalette(new int[] { this.Value, newState });
        }
    }
}
