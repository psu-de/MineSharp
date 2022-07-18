using MineSharp.Data.Protocol;
using MineSharp.Protocol;

namespace MineSharp.World.PalettedContainer.Palettes {
    public interface IPalette {
        public int Get(int entry);
        //public IPalette Set(int index, int value);
        public void Read(PacketBuffer buffer);
        public bool HasState(int minState, int maxState);
    }
}
