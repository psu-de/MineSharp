using MineSharp.ConsoleClient.Client;
using Spectre.Console;

namespace MineSharp.ConsoleClient.Console.Commands.Player
{
    internal class RespawnCommand : Command
    {

        public RespawnCommand()
        {

            var desc = "Respawns the Bot if it's dead";

            this.Initialize("respawn", desc, CColor.PlayerCommand);
        }

        public override void DoAction(string[] argv, CancellationToken cancellation)
        {
            try
            {

                BotClient.Bot!.Respawn();
            } catch (Exception)
            {
                AnsiConsole.MarkupLine("[red]Player is not dead![/]");
            }
        }
    }
}
