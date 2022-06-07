using System;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Core.Types {
    public class IntBitArray {



        public long[] Data { get; private set; }
        public byte BitsPerEntry { get; private set; }
        public int Capacity => (Data.Length * 64) / BitsPerEntry;

        private int ValuesPerLong => 64 / BitsPerEntry;
        private long ValueMask => (1L << BitsPerEntry) - 1;

        public IntBitArray(long[] data, byte bitsPerEntry) {
            this.Data = data;
            this.BitsPerEntry = bitsPerEntry;

            if (bitsPerEntry == 0) return;
        }

        public void Set(int idx, int value) {

            if (idx >= Capacity || idx < 0) throw new ArgumentOutOfRangeException(nameof(idx));

            int longIndex = idx / ValuesPerLong;
            int bitIndex = (idx % ValuesPerLong) * BitsPerEntry;

            long l = Data[longIndex];

            long shiftedValue = (value & ValueMask) << bitIndex;

            l |= shiftedValue;

            long mask = ~(ValueMask << bitIndex) | shiftedValue;
            l &= mask;

            Data[longIndex] = l;
        }

        public int Get(int idx) {

            if (idx >= Capacity || idx < 0) throw new ArgumentOutOfRangeException(nameof(idx));

            int longIndex = idx / ValuesPerLong;
            int bitIndex = (idx % ValuesPerLong) * BitsPerEntry;

            long l = Data[longIndex];
            return (int)((l >> bitIndex) & ValueMask);
        }
    }
}
