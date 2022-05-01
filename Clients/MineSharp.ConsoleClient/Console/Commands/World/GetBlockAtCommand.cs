using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using PrettyPrompt.Highlighting;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.ConsoleClient.Console.Commands.World {
    internal class GetBlockAtCommand : Command {

        IntegerArgument X = new IntegerArgument("x");
        IntegerArgument Y = new IntegerArgument("y");
        IntegerArgument Z = new IntegerArgument("z");

        public GetBlockAtCommand() {

            string desc = $"Gets a [purple]block[/] at the [{X.Color}]x y z[/] coordinates";

            this.Initialize("getBlockAt", desc, CColor.WorldCommand, X, Y, Z);
        }

        public override void DoAction(string[] argv, CancellationToken cancellation) {

            int? x = X.GetValue(argv[0]);
            int? y = Y.GetValue(argv[1]);
            int? z = Z.GetValue(argv[2]);

            if (x == null || y == null || z == null) {
                AnsiConsole.MarkupLine($"[red]Error: Coordinates invalid");
                return;
            }

            var block = BotClient.Bot.GetBlockAt(new Core.Types.Position((int)x, (int)y, (int)z));
            AnsiConsole.MarkupLine($"[green]{block}[/]");
        }
    }
}
