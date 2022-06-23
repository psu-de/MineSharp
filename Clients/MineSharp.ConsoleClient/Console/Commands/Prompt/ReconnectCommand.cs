using MineSharp.ConsoleClient.Client;
using Spectre.Console;

namespace MineSharp.ConsoleClient.Console.Commands.Prompt {
    internal class ReconnectCommand : Command {

        public ReconnectCommand() {
            var desc = $"Reconnects the bot";
            this.Initialize("reconnect", desc, CColor.PromptCommand, new Commands.Arguments.Argument[] { });
        }
        public override void DoAction(string[] argv, CancellationToken cancellation) {
            var options = BotClient.Bot!.Options;
            BotClient.Bot.Client.ForceDisconnect("");
            BotClient.Bot = new Bot.MinecraftBot(options);
            if (!BotClient.Bot.Connect().GetAwaiter().GetResult()) {
                AnsiConsole.MarkupLine("[red]Error reconnecting![/]");
                Environment.Exit(0);
            }
        }
    }
}
