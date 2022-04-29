using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using PrettyPrompt.Highlighting;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.ConsoleClient.Console.Commands.Prompt {
    internal class ShowCommand : Command {

        EnumArgument<ShowCommandOption> OptionArgument;

        public ShowCommand() {

            OptionArgument = new EnumArgument<ShowCommandOption>("option");
            var desc = $"Shows a table of the specified [{OptionArgument.Color}]option[/]";


            this.Initialize("show", desc, CColor.PromptCommand, OptionArgument);
        }

        public override void PrintHelp() {
            base.PrintHelp();

            AnsiConsole.MarkupLine($"Possible Options: ");
            var table = new Table()
                .AddColumn("Options");

            foreach (var s in Enum.GetNames(typeof(ShowCommandOption))) {
                table.AddRow(s);
            }

            AnsiConsole.Write(table);
        }

        public override void DoAction(string[] argv, CancellationToken cancellation) {

            ShowCommandOption option = OptionArgument.GetValue(argv[0]);

            Table table = new Table();

            switch (option) {
                case ShowCommandOption.Commands:
                    table.AddColumns("Commands", "Help");
                    foreach (var c in CommandManager.Commands.Values.ToArray()) {
                        table.AddRow(new Text(c.Name), new Panel(c.Description));
                    }
                    break;
            }
            if (table.Rows.Count > 0) {
                AnsiConsole.Write(table);
            } else {
                AnsiConsole.MarkupLine($"[red]Nothing found![/]");
            }

        }

        enum ShowCommandOption {
            Commands,
        }
    }
}
