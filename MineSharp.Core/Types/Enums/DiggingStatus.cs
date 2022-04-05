using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Core.Types.Enums {
    public enum DiggingStatus {
        StartedDigging = 0,
        CancelledDigging = 1,
        FinishedDigging = 2,
        DropItemStack = 3,
        DropItem = 4,
        ShootArrow_FinishedEating = 5,
        SwapItemHand = 6
    }
}
