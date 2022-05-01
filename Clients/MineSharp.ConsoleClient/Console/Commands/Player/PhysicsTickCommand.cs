using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.ConsoleClient.Console.Commands.Player {
    internal class PhysicsTickCommand : Command {

        public PhysicsTickCommand() {
            string desc = $"Ticks Physics engine once, should not be used when Physics is already enabled";
            this.Initialize("physicTick", desc, CColor.PlayerCommand, new Argument[] { });
        }

        public override void DoAction(string[] argv, CancellationToken cancellation) {

            BotClient.Bot.Physics.SimulatePlayer(BotClient.Bot.MovementControls);
        }
    }
}
