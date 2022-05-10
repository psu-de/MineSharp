using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.ConsoleClient.Console.Commands.Player {
    internal class RaycastCommand : Command {

        IntegerArgument LengthArgument;
        public RaycastCommand() {
            LengthArgument = new IntegerArgument("length", true);

            var desc = $"Tries to figure out at what block the bot is looking";
            this.Initialize("raycast", desc, CColor.PlayerCommand, LengthArgument);
        }

        public override void DoAction(string[] argv, CancellationToken cancellation) {

            var length = 100;
            if (argv.Length > 0) {
                length = LengthArgument.GetValue(argv[0]) ?? throw new Exception("Argument invalid");
            }

            var block = BotClient.Bot.Raycast(length).GetAwaiter().GetResult();
            if (block != null) {
                AnsiConsole.MarkupLine($"[green]{block}[/]");
            } else {
                AnsiConsole.MarkupLine("[red]Nothing found![/]");
            }
        }
    }
}
