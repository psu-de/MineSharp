using MineSharp.Core.Types;
using MineSharp.Data.Protocol;
using MineSharp.World.PalettedContainer.Palettes;

namespace MineSharp.World.PalettedContainer {
    public interface IPalettedContainer {

        public IPalette Palette { get; set; }
        public int Capacity { get; }
        public IntBitArray Data { get; set; }

        public static void Read(PacketBuffer buffer) {
            throw new NotImplementedException();
        }

        public int GetAt(int index);

        public void SetAt(int index, int state);
    }
}
