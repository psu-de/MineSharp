using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Core.Types {
    public class BitArray {

        public long[] Data { get; private set; }
        public int Capacity { get; private set; }
        public byte BitsPerEntry { get; private set; }
        private int ValueMask;
        private int ValuesPerLong;

        public BitArray(long[] data, int capacity, byte bitsPerEntry) {
            this.Data = data;
            this.Capacity = capacity;
            this.BitsPerEntry = bitsPerEntry;

            if (BitsPerEntry != 0) {
                this.ValueMask = (1 << BitsPerEntry) - 1;
                this.ValuesPerLong = (int)Math.Ceiling((float)(64 / this.BitsPerEntry));
            }
        }


        public void Set(int index, int value) {
            if (index < 0 || index > this.Capacity) throw new ArgumentOutOfRangeException();

            
            int longIndex = index / this.ValuesPerLong;
            int bitIndex = (index - longIndex * this.ValuesPerLong) * this.BitsPerEntry;

            long masked = (value & this.ValueMask);

            if (bitIndex > 64) {
                int indexInStartLong = bitIndex - 64;
                long startLong = this.Data[bitIndex + 1];
                masked <<= indexInStartLong;
                startLong &= masked;
                startLong |= masked;
                this.Data[bitIndex + 1] = startLong;
            } else {

                long startLong = this.Data[longIndex];
                int indexInStartLong = bitIndex;
                var v = masked << indexInStartLong;
                startLong &= v;
                startLong |= v;
                this.Data[longIndex] = startLong;

                int endBitOffset = indexInStartLong + this.BitsPerEntry;

                if (endBitOffset > 64) {
                    long endLong = this.Data[longIndex + 1];
                    v = masked << (64 - indexInStartLong);
                    endLong &= v;
                    endLong |= v;
                    this.Data[longIndex + 1] = endLong;
                }

            }
        }

        public int Get(int index) {
            if (index < 0 || index > this.Capacity) throw new ArgumentOutOfRangeException();
            long result;

            int longIndex = index / this.ValuesPerLong;
            int bitIndex = (index - longIndex * this.ValuesPerLong) * this.BitsPerEntry;
            if (bitIndex > 64) {
                int indexInStartLong = bitIndex - 64;
                long startLong = this.Data[bitIndex + 1];
                result = (int)((startLong >> indexInStartLong) & this.ValueMask);
            } else {

                long startLong = this.Data[longIndex];
                int indexInStartLong = bitIndex;
                result = startLong >> indexInStartLong;
                int endBitOffset = indexInStartLong + this.BitsPerEntry;

                if (endBitOffset > 64) {
                    long endLong = this.Data[longIndex + 1];
                    result |= endLong << (64 - indexInStartLong);
                }
                result &= this.ValueMask;

            }
            return (int)result;
        }
    }
}
