using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Protocol {
    public enum GameState {

        HANDSHAKING = -1,
        STATUS = 1,
        LOGIN = 2, 
        PLAY = 0
    }
}
