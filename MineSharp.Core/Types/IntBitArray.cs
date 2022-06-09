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
        public int Capacity => ValuesPerLong * Data.Length;

        private int ValuesPerLong => 64 / BitsPerEntry;
        private long ValueMask => (1L << BitsPerEntry) - 1;

        public IntBitArray(long[] data, byte bitsPerEntry) {
            this.Data = data;
            this.BitsPerEntry = bitsPerEntry;
        }

        public void ChangeBitsPerEntry(byte newBitsPerEntry) {
            if (newBitsPerEntry == BitsPerEntry) return;

            var old = new IntBitArray(this.Data, this.BitsPerEntry);
            var capacity = this.Capacity;
            this.BitsPerEntry = newBitsPerEntry;

            this.Data = new long[(int)Math.Ceiling((float)capacity / ValuesPerLong)];
            for (int i = 0; i < capacity; i++) {
                this.Set(i, old.Get(i));
            }
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
