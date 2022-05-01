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
            CommandNameArg = new CommandNameArgument("command");

            var desc = $"Displays [{CColor.PromptCommand}]help[/] for another [{CommandNameArg.Color}]command[/]";


            this.Initialize("help", desc, CColor.PromptCommand, CommandNameArg);
        }

        public override void DoAction(string[] argv, CancellationToken cancellation) {
            string commandName = argv[0];

            if(!CommandManager.TryGetCommand(commandName, out var command)) {
                AnsiConsole.MarkupLine("[red] ERROR: Could not get command " + commandName + "[/]");
            }

            command.PrintHelp();
        }
    }
}
