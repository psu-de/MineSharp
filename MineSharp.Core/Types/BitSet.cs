namespace MineSharp.Core.Types {
    public class BitSet {

        public long[] Values { get; private set; }

        public BitSet(long[] values) {
            this.Values = values;
        }


        public bool GetBit(int index) {
            return (this.Values[index / 64] & (1L << (index % 64))) != 0;
        }
    }
}
