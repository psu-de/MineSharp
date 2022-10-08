using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using MineSharp.Pathfinding;
using MineSharp.Pathfinding.Goals;
using Spectre.Console;

namespace MineSharp.ConsoleClient.Console.Commands.Player
{
    internal class PathfindCommand : Command
    {
        private IntegerArgument X = new IntegerArgument("x");
        private IntegerArgument Y = new IntegerArgument("y");
        private IntegerArgument Z = new IntegerArgument("z");

        private FloatArgument Timeout = new FloatArgument("timeout", true);
        public PathfindCommand()
        {
            var desc = $"Tries to find a path to the [{this.X.Color}]x y z[/] coordinates";
            this.Initialize("pathfind", desc, CColor.PlayerCommand, this.X, this.Y, this.Z, this.Timeout);
        }

        public override async void DoAction(string[] argv, CancellationToken cancellation)
        {
            var x = this.X.GetValue(argv[0]);
            var y = this.Y.GetValue(argv[1]);
            var z = this.Z.GetValue(argv[2]);

            float? timeout = null;
            if (argv.Length > 3)
                timeout = this.Timeout.GetValue(argv[3]);

            if (x == null || y == null || z == null)
            {
                AnsiConsole.MarkupLine($"[red]Error: Coordinates invalid[/]");
                return;
            }

            var goal = new GoalXYZ(x.Value, y.Value, z.Value);
            var pathfinder = new PathfinderModule(BotClient.Bot!);
            await pathfinder.Initialize();
            AnsiConsole.Status()
                .Start("Pathfinding...", ctx =>
                {
                    try
                    {
                        pathfinder.GoTo(goal, timeout: timeout ?? 10000, cancellation: cancellation).GetAwaiter().GetResult();
                    } catch (Exception e)
                    {
                        AnsiConsole.WriteException(e);
                    }
                });
        }
    }
}
