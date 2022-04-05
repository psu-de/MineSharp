using MineSharp.Protocol.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.World.PalettedContainer.Palettes {
    internal class DirectPalette : IPalette {
        public int Get(int entry) {
            return entry;
        }

        public bool HasState(int minState, int maxState) {
            return true;
        }

        public void Read(PacketBuffer buffer) {
        }
    }
}
