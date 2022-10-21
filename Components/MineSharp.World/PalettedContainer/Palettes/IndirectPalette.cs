using MineSharp.Data.Protocol;

namespace MineSharp.World.PalettedContainer.Palettes
{
    internal class IndirectPalette : IPalette
    {

        internal const int BLOCK_MAX_BITS = 8;
        internal const int BLOCK_MIN_BITS = 4;
        internal const int BIOME_MAX_BITS = 3;
        internal const int BIOME_MIN_BITS = 1;

        public int[]? Map;

        public IndirectPalette() {}

        public IndirectPalette(int[] map)
        {
            this.Map = map;
        }

        public int Get(int entry) => this.Map![entry];

        public bool HasState(int minState, int maxState)
        {

            for (var i = 0; i < this.Map!.Length; i++)
            {
                if (minState <= this.Map[i] && this.Map[i] <= maxState) return true;
            }

            return false;
        }

        public void Read(PacketBuffer buffer)
        {
            this.Map = buffer.ReadArray<int>(buffer.ReadVarInt(), buffer => buffer.ReadVarInt()); // TODO: Varint
        }

        public int GetStateIndex(int state) => this.Map!.ToList().IndexOf(state);

        public IPalette AddState(int state, bool biomes, out byte newBitsPerEntry)
        {
            if (this.HasState(state, state)) throw new ArgumentException("Palette already contains state");

            var newMapSize = this.Map!.Length + 1;
            newBitsPerEntry = (byte)Math.Ceiling(Math.Log2(newMapSize));
            newBitsPerEntry = (byte)Math.Max(newBitsPerEntry, biomes ? BIOME_MIN_BITS : BLOCK_MIN_BITS);

            if (newBitsPerEntry > (biomes ? BIOME_MAX_BITS : BLOCK_MAX_BITS))
            {
                // direct palette neeeded
                return new DirectPalette();
            }
            var newMap = this.Map.ToList();
            newMap.Add(state);
            return new IndirectPalette(newMap.ToArray());
        }
    }
}
