using MineSharp.Data.Blocks;
using MineSharp.Protocol.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.World.PalettedContainer.Palettes {
    public interface IPalette {
        public int Get(int entry);
        //public IPalette Set(int index, int value);
        public void Read(PacketBuffer buffer);
        public bool HasState(int minState, int maxState);
    }
}
