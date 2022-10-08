using MineSharp.Bot.Modules.Physics;
using MineSharp.Core.Types;
using MineSharp.Physics;

namespace MineSharp.Bot
{
    public partial class MinecraftBot
    {


        public PhysicsEngine? Physics => this.PhysicsModule!.Physics;
        public PlayerControls PlayerControls => this.PhysicsModule!.PlayerControls;

        /// <summary>
        ///     Fires when the Bot <see cref="BotEntity" /> moves
        /// </summary>
        public event BotPlayerEvent BotMoved {
            add => this.PhysicsModule!.BotMoved += value;
            remove => this.PhysicsModule!.BotMoved -= value;
        }

        /// <summary>
        ///     Fires just before a physics tick
        /// </summary>
        public event BotEmptyEvent PhysicsTick {
            add => this.PhysicsModule!.PhysicsTick += value;
            remove => this.PhysicsModule!.PhysicsTick -= value;
        }

        [BotFunction("Physics", "Sets the bots rotation to a given yaw and pitch")]
        public void ForceSetRotation(float yaw, float pitch) => this.PhysicsModule!.ForceSetRotation(yaw, pitch);

        [BotFunction("Physics", "Forces the bot to look at a given position")]
        public void ForceLookAt(Position pos) => this.PhysicsModule!.ForceLookAt(pos);

        [BotFunction("physics", "Waits until the bot has reached the position")]
        public Task WaitUntilReached(Vector3 pos) => this.PhysicsModule!.WaitUntilReached(pos);
    }
}
