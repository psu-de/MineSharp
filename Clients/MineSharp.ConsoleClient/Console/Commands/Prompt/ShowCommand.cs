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

            switch (option) {
                case ShowCommandOption.Commands:
                    PrintCommands();
                    break;
            }

        }

        void PrintCommands() {
            Dictionary<string, List<Command>> commands = new Dictionary<string, List<Command>>();
            foreach (var c in CommandManager.Commands.Values) {
                var commandType = c.GetType().Namespace?.Split('.').Last() ?? "Unknown";
                commandType += " Commands";
                if (commands.ContainsKey(commandType)) {
                    commands[commandType].Add(c);
                } else {
                    commands[commandType] = new List<Command>() { c };
                }
            }

            foreach (var group in commands) {
                var table = new Table().AddColumns(group.Key, "Help");
                foreach (var c in group.Value) {
                    table.AddRow(new Markup($"\n[{c.Color}]{c.Name}[/]"), new Panel(c.Description));
                }
                AnsiConsole.Write(table);
                AnsiConsole.WriteLine();
            }
        }

        enum ShowCommandOption {
            Commands,
        }
    }
}
