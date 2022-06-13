using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using PrettyPrompt;
using PrettyPrompt.Highlighting;
using Spectre.Console;

namespace MineSharp.ConsoleClient.Console.Commands.Prompt {
    internal class HelpCommand : Command {

        CommandNameArgument CommandNameArg;

        public HelpCommand() {
            CommandNameArg = new CommandNameArgument("command", isOptional: true);

            var desc = $"Lists all commands or displays [{CColor.PromptCommand}]help[/] for another [{CommandNameArg.Color}]command[/]";


            this.Initialize("help", desc, CColor.PromptCommand, CommandNameArg);
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

            Table masterTable = new Table().AddColumns("Category", "");

            foreach (var group in commands) {
                var table = new Table().AddColumns("Command", "Help");
                foreach (var c in group.Value) {
                    table.AddRow(new Markup($"\n[{c.Color}]{c.Name}[/]"), new Panel(c.Description));
                }
                masterTable.AddRow(new Text("\n" + group.Key), table.Width(1000));
            }

            AnsiConsole.Write(masterTable);
            AnsiConsole.WriteLine();
        }

        public override void DoAction(string[] argv, CancellationToken cancellation) {

            if (argv.Length == 0) {
                PrintCommands();
                return;
            }

            string commandName = argv[0];

            if(!CommandManager.TryGetCommand(commandName, out var command)) {
                AnsiConsole.MarkupLine("[red] ERROR: Could not find command " + commandName + "[/]");
            }

            command.PrintHelp();
        }
    }
}
