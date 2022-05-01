using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using MineSharp.Core.Types;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.ConsoleClient.Console.Commands.Player {
    internal class LookAtCommand : Command {

        IntegerArgument X;
        IntegerArgument Y;
        IntegerArgument Z;

        public LookAtCommand() {
            X = new IntegerArgument("x");
            Y = new IntegerArgument("y");
            Z = new IntegerArgument("y");

            var desc = $"Makes the Player look at the specified coordinates [{X.Color}]X Y Z[/]";
            this.Initialize("lookAt", desc, CColor.PlayerCommand, X, Y, Z);
        }

        public override void DoAction(string[] argv, CancellationToken cancellation) {

            int? x = X.GetValue(argv[0]);
            int? y = Y.GetValue(argv[1]);
            int? z = Y.GetValue(argv[2]);

            if (x == null || y == null || z == null) {
                AnsiConsole.MarkupLine($"[{CColor.Error}]Invalid pitch![/]");
            }

            BotClient.Bot.ForceLookAt(new Position(x.Value, y.Value, z.Value));
        }

    }
}
