using MineSharp.Data.Protocol;

namespace MineSharp.World.PalettedContainer.Palettes
{
    internal class SingleValuePalette : IPalette
    {

        public int Value;

        public int Get(int entry) => this.Value;

        public bool HasState(int minState, int maxState) => minState <= this.Value && this.Value <= maxState;

        public void Read(PacketBuffer buffer)
        {
            this.Value = buffer.ReadVarInt();
        }

        public IndirectPalette ConvertToIndirectPalette(int newState)
        {
            return new IndirectPalette(new[] {
                this.Value, newState
            });
        }
    }
}
