using MineSharp.Data.Protocol;
namespace MineSharp.World.PalettedContainer.Palettes
{
    internal class DirectPalette : IPalette
    {
        public int Get(int entry) => entry;

        public bool HasState(int minState, int maxState) => true;

        public void Read(PacketBuffer buffer) {}
    }
}
