using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using MineSharp.Pathfinding;
using MineSharp.Pathfinding.Goals;
using Spectre.Console;

namespace MineSharp.ConsoleClient.Console.Commands.Player
{
    internal class PathfindCommand : Command
    {
        IntegerArgument X = new IntegerArgument("x");
        IntegerArgument Y = new IntegerArgument("y");
        IntegerArgument Z = new IntegerArgument("z");

        FloatArgument Timeout = new FloatArgument("timeout", true);
        public PathfindCommand()
        {
            string desc = $"Tries to find a path to the [{X.Color}]x y z[/] coordinates";
            this.Initialize("pathfind", desc, CColor.PlayerCommand, X, Y, Z, Timeout);
        }

        public override async void DoAction(string[] argv, CancellationToken cancellation)
        {
            int? x = X.GetValue(argv[0]);
            int? y = Y.GetValue(argv[1]);
            int? z = Z.GetValue(argv[2]);

            float? timeout = null;
            if (argv.Length > 3) 
                timeout = Timeout.GetValue(argv[3]);

            if (x == null || y == null || z == null)
            {
                AnsiConsole.MarkupLine($"[red]Error: Coordinates invalid[/]");
                return;
            }

            var goal = new GoalXYZ(x.Value, y.Value, z.Value);
            var pathfinder = new PathfinderModule(BotClient.Bot!);
            await pathfinder.Initialize();
            AnsiConsole.Status()
                .Start("Pathfinding...", ctx => {
                    try
                    {
                        pathfinder.GoTo(goal, timeout: timeout ?? 10000).Wait(cancellation);
                    } catch (Exception e)
                    {
                        AnsiConsole.WriteException(e);
                    }
                });
        }
    }
}
