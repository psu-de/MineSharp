
using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.ConsoleClient.Console.Commands.Chat {
    internal class SayCommand : Command {

        StringArgument Message;

        public SayCommand() {

            Message = new StringArgument("message");
            var desc = $"Writes a [{Message.Color}]message[/] into the chat";

            this.Initialize("say", desc, CColor.ChatCommand, Message);
        }

        public override void DoAction(string[] argv, CancellationToken cancellation) {
            BotClient.Bot.Chat(Message.GetValue(argv[0]));
            AnsiConsole.MarkupLine("[green] Wrote message " + Message.GetValue(argv[0]) + "[/]");
        }
    }
}
