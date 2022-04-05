using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Core.Types {
    public class Position {

        public int X { get; private set; }
        public int Y { get; private set; }
        public int Z { get; private set; }

        public Position(ulong value) {
            this.X = (int)(value >> 38);
            this.Y = (int)(value & 0xFFF);
            this.Z = (int)((value >> 12) & 0x3FFFFFF);

            if (this.X >= Math.Pow(2, 25)) { this.X -= (int)Math.Pow(2, 26); }
            if (this.Y >= Math.Pow(2, 11)) { this.Y -= (int)Math.Pow(2, 12); }
            if (this.Z >= Math.Pow(2, 25)) { this.Z -= (int)Math.Pow(2, 26); }
        }

        public Position(int x, int y, int z) {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public ulong ToULong() {
            return (((ulong)this.X & 0x3FFFFFF) << 38) | (((ulong)this.Z & 0x3FFFFFF) << 12) | ((ulong)this.Y & 0xFFF);
        }

        public override string ToString() {
            return $"({X} / {Y} / {Z})";
        }
    }
}
