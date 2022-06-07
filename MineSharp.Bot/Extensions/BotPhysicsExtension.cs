using MineSharp.Core;
using MineSharp.Core.Types;
using MineSharp.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Bot {
    public partial class MinecraftBot {

        /// <summary>
        /// Fires when the Bot <see cref="BotEntity"/> moves
        /// </summary>
        public BotPlayerEvent BotMoved;


        public Physics.Physics Physics => PhysicsModule.Physics;
        public PlayerControls MovementControls => PhysicsModule.MovementControls;
        
        public void ForceSetRotation(float yaw, float pitch) => this.PhysicsModule.ForceSetRotation(yaw, pitch);
        public void ForceLookAt(Position pos) => this.PhysicsModule.ForceLookAt(pos);
    }
}
