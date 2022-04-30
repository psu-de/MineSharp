using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.ConsoleClient.Console.Commands.Misc {
    internal class EnablePhysicsCommand : Command {

        BoolArgument EnabledArg = new BoolArgument("enabled");

        public EnablePhysicsCommand() {
            var desc = $"Enables or disables the Physics engine";
            this.Initialize("enablePhysics", desc, CColor.MiscCommand, EnabledArg);

        }

        public override void DoAction(string[] argv, CancellationToken cancellation) {
            bool enabled = EnabledArg.GetValue(argv[0]);
            BotClient.Bot.PhysicsEnabled = enabled;
        }
    }
}
