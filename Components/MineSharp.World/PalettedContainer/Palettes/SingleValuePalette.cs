using MineSharp.Data.Blocks;
using MineSharp.Protocol.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.World.PalettedContainer.Palettes {
    internal class SingleValuePalette : IPalette {

        public int Value;

        public int Get(int entry) {
            return Value;
        }

        public bool HasState(int minState, int maxState) {
            return minState <= Value && Value <= maxState;
        }

        public void Read(PacketBuffer buffer) {
            this.Value = buffer.ReadVarInt();
        }
    }
}
