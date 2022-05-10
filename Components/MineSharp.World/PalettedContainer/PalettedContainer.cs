using MineSharp.Core.Logging;
using MineSharp.Core.Types;
using MineSharp.Data.Blocks;
using MineSharp.Protocol.Packets;
using MineSharp.World.PalettedContainer.Palettes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.World.PalettedContainer {
    public class PalettedContainer {

        private const int INDIRECT_BLOCK_MAX_BITS = 8;
        private const int INDIRECT_BLOCK_MIN_BITS = 4; 

        private static Logger Logger = Logger.GetLogger();

        public bool Biomes { get; set; }
        public int Capacity { get; set; }
        public IPalette Palette { get; set; }
        public BitArray Data { get; set; }
        
        public PalettedContainer(bool biomes, int capacity) {
            this.Biomes = biomes;
            this.Capacity = capacity;
        }

        private IPalette GetPalette(byte bits) {
            if (bits == 0) return new SingleValuePalette();
            else if ((!Biomes && bits > 8) || (Biomes && bits > 3)) return new DirectPalette();
            else return new IndirectPalette();
        }

        public void Read(PacketBuffer buffer) {
            byte bitsPerEntry = buffer.ReadByte();
            this.Palette = GetPalette(bitsPerEntry);
            this.Palette.Read(buffer);

            long[] data = new long[buffer.ReadVarInt()];
            for (int i = 0; i < data.Length; i++) data[i] = buffer.ReadLong();

            this.Data = new BitArray(data, this.Capacity, bitsPerEntry);
        }

        public int GetAt(int index) {

            if (Palette.GetType() == typeof(SingleValuePalette)) {
                return Palette.Get(0);
            }

            return this.Palette.Get(this.Data.Get(index));
        }

        public void SetAt (int index, int state) {

            if (this.Biomes) throw new NotSupportedException();

            if (this.Palette.HasState(state, state)) {
                switch (Palette) {
                    case SingleValuePalette svp: break;
                    case IndirectPalette ip:
                        int entry = ip.GetStateIndex(state);
                        this.Data.Set(index, entry);
                        break;
                    case DirectPalette dp:
                        this.Data.Set(index, state);
                        break;
                }
            } else {

                byte newBitsPerEntry;
                BitArray newData; 
                int dataLength;
                switch (Palette) {
                    case SingleValuePalette svp:
                        newBitsPerEntry = INDIRECT_BLOCK_MIN_BITS;
                        dataLength = (int)Math.Ceiling((float)((float)this.Capacity / (64 / newBitsPerEntry)));
                        newData = new BitArray(new long[dataLength], this.Capacity, newBitsPerEntry);
                        newData.Set(index, 1);
                        this.Palette = new IndirectPalette(new int[] { svp.Value, state });
                        this.Data = newData;
                        break;
                    case IndirectPalette ip:

                        int newMapSize = ip.Map.Length + 1;
                        Array.Resize(ref ip.Map, newMapSize);
                        ip.Map[newMapSize - 1] = state;

                        newBitsPerEntry = (byte)Math.Ceiling(Math.Log2(newMapSize));
                        if (newBitsPerEntry < INDIRECT_BLOCK_MIN_BITS) newBitsPerEntry = INDIRECT_BLOCK_MIN_BITS;

                        if (newBitsPerEntry != this.Data.BitsPerEntry) {
                            if (newBitsPerEntry <= INDIRECT_BLOCK_MAX_BITS) {
                                dataLength = (int)Math.Ceiling((float)((float)this.Capacity / (64 / newBitsPerEntry)));
                                newData = new BitArray(new long[dataLength], this.Capacity, newBitsPerEntry);
                                for (int i = 0; i < this.Capacity; i++) {
                                    int s = this.Data.Get(i);
                                    newData.Set(i, s);
                                }
                                this.Data = newData;
                            } else {
                                // Convert to direct palette
                                Logger.Debug("Converting to direct palette"); //TODO: Scheint noch nich ganz zu funktionieren
                                newBitsPerEntry = (byte)Math.Ceiling(Math.Log2(BlockData.StateToBlockMap.Count));

                                dataLength = (int)Math.Ceiling((float)this.Capacity / (64 / newBitsPerEntry));
                                newData = new BitArray(new long[dataLength], this.Capacity, newBitsPerEntry);
                                for (int i = 0; i < this.Capacity; i++) {
                                    int s = this.Palette.Get(this.Data.Get(i));
                                    newData.Set(i, s);
                                }
                                this.Palette = new DirectPalette();
                                this.Data = newData;
                                break;
                            }
                        }

                        this.Data.Set(index, newMapSize - 1);

                        break;
                }
            }

        }
    }
}
