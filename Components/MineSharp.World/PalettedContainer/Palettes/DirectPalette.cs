using MineSharp.Protocol.Packets;

namespace MineSharp.World.PalettedContainer.Palettes {
    internal class DirectPalette : IPalette {
        public int Get(int entry) {
            return entry;
        }

        public bool HasState(int minState, int maxState) {
            return true;
        }

        public void Read(PacketBuffer buffer) {
        }
    }
}
