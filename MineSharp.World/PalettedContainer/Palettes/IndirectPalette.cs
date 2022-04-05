using MineSharp.Protocol.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.World.PalettedContainer.Palettes {
    internal class IndirectPalette : IPalette {

        public int[] Map;

        public IndirectPalette() { }

        public IndirectPalette(int[] map) {
            this.Map = map;
        }

        public int Get(int entry) {
            return Map[entry];
        }

        public int GetStateIndex(int state) {
            return Map.ToList().IndexOf(state);
        }

        public bool HasState(int minState, int maxState) {

            for (int i = 0; i < Map.Length; i++) {
                if (minState <= Map[i] && Map[i] <= maxState) return true;
            }

            return false;
        }

        public void Read(PacketBuffer buffer) {
            this.Map = buffer.ReadVarIntArray();
        }
    }
}
