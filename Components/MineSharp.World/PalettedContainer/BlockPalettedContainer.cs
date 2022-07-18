using MineSharp.Core.Logging;
using MineSharp.Core.Types;
using MineSharp.Data.Protocol;
using MineSharp.Protocol;
using MineSharp.World.PalettedContainer.Palettes;

namespace MineSharp.World.PalettedContainer {
    public class BlockPalettedContainer : IPalettedContainer {

        static Logger Logger = Logger.GetLogger();

        public static BlockPalettedContainer Read(PacketBuffer buffer) {
            byte bitsPerEntry = buffer.ReadU8();
            var palette = GetPalette(bitsPerEntry);
            palette.Read(buffer);

            long[] data = new long[buffer.ReadVarInt()];
            for (int i = 0; i < data.Length; i++) data[i] = buffer.ReadI64();

            return new BlockPalettedContainer(palette, new IntBitArray(data, bitsPerEntry));
        }

        private static IPalette GetPalette(byte bitsPerEntry) => bitsPerEntry switch {
            0 => new SingleValuePalette(),
            <= IndirectPalette.BLOCK_MAX_BITS => new IndirectPalette(),
            _ => new DirectPalette()
        };

        public IPalette Palette { get; set; }
        public int Capacity => 16 * 16 * 16;
        public IntBitArray Data { get; set; }

        public BlockPalettedContainer(IPalette palette, IntBitArray data) { 
            Palette = palette;
            Data = data;
        }

        public int GetAt(int index) {
            if (index < 0 || index >= Capacity) 
                throw new ArgumentOutOfRangeException(nameof(index));

            if (this.Palette is SingleValuePalette)
                return this.Palette.Get(0);

            var value = Data.Get(index);
            return this.Palette.Get(value);
        }

        public void SetAt(int index, int state) {

            if (index < 0 || index >= Capacity)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (this.Palette.HasState(state, state)) {
                switch (this.Palette) {
                    case SingleValuePalette svp: break;
                    case IndirectPalette ip:
                        var mapIndex = ip.GetStateIndex(state);
                        this.Data.Set(index, mapIndex);
                        break;
                    case DirectPalette dp:
                        this.Data.Set(index, state);
                        break;
                }
            } else {

                switch (this.Palette) {
                    case SingleValuePalette svp:
                        Logger.Info($"Converting {nameof(SingleValuePalette)} to {nameof(IndirectPalette)}");
                        this.Palette = svp.ConvertToIndirectPalette(state);
                        this.Data = new IntBitArray(new long[(int)Math.Ceiling((float)this.Capacity / (64 / IndirectPalette.BLOCK_MIN_BITS))], IndirectPalette.BLOCK_MIN_BITS);
                        this.Data.Set(index, 1);
                        break;
                    case IndirectPalette dp:
                        var newPalette = dp.AddState(state, false, out var newBitsPerEntry);
                        Logger.Info($"Converting {nameof(IndirectPalette)} (bps={Data.BitsPerEntry}) to {newPalette.GetType().Name} (bps={newBitsPerEntry})");
                        this.Data.ChangeBitsPerEntry(newBitsPerEntry);

                        if (newPalette is DirectPalette) {
                            for (int i = 0; i < Capacity; i++) {
                                this.Data.Set(i, GetAt(i));
                            }
                        }

                        if (newPalette is DirectPalette)
                            this.Data.Set(index, state);
                        else 
                            this.Data.Set(index, ((IndirectPalette)newPalette).GetStateIndex(state));
                        this.Palette = newPalette;
                        break;
                }

            }

        }
    }
}
