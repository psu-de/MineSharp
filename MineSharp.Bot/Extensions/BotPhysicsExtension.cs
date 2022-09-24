using MineSharp.Bot.Modules.Physics;
using MineSharp.Core.Types;
using MineSharp.Physics;

namespace MineSharp.Bot {
    public partial class MinecraftBot {

        /// <summary>
        /// Fires when the Bot <see cref="BotEntity"/> moves
        /// </summary>
        public event BotPlayerEvent BotMoved {
            add { PhysicsModule!.BotMoved += value; }
            remove { PhysicsModule!.BotMoved -= value; }
        }


        public Physics.PhysicsEngine? Physics => PhysicsModule!.Physics;
        public PlayerControls PlayerControls => PhysicsModule!.PlayerControls;

        [BotFunction("Physics", "Sets the bots rotation to a given yaw and pitch")]
        public void ForceSetRotation(float yaw, float pitch) => this.PhysicsModule!.ForceSetRotation(yaw, pitch);

        [BotFunction("Physics", "Forces the bot to look at a given position")]
        public void ForceLookAt(Position pos) => this.PhysicsModule!.ForceLookAt(pos);
    }
}
