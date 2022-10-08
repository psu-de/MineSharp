using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using Spectre.Console;

namespace MineSharp.ConsoleClient.Console.Commands.Chat
{
    internal class SayCommand : Command
    {

        private StringArgument Message;

        public SayCommand()
        {

            this.Message = new StringArgument("message");
            var desc = $"Writes a [{this.Message.Color}]message[/] into the chat";

            this.Initialize("say", desc, CColor.ChatCommand, this.Message);
        }

        public override void DoAction(string[] argv, CancellationToken cancellation)
        {
            BotClient.Bot!.Chat(this.Message.GetValue(argv[0]));
            AnsiConsole.MarkupLine("[green] Wrote message " + this.Message.GetValue(argv[0]) + "[/]");
        }
    }
}
