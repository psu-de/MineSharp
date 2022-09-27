using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using MineSharp.Pathfinding;
using MineSharp.Pathfinding.Algorithm;
using MineSharp.Pathfinding.Goals;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.ConsoleClient.Console.Commands.Player
{
    internal class PathfindCommand : Command
    {
        IntegerArgument X = new IntegerArgument("x");
        IntegerArgument Y = new IntegerArgument("y");
        IntegerArgument Z = new IntegerArgument("z");
        public PathfindCommand()
        {
            string desc = $"Tries to find a path to the [{X.Color}]x y z[/] coordinates";
            this.Initialize("pathfind", desc, CColor.PlayerCommand, X, Y, Z);
        }

        public override async void DoAction(string[] argv, CancellationToken cancellation)
        {
            int? x = X.GetValue(argv[0]);
            int? y = Y.GetValue(argv[1]);
            int? z = Z.GetValue(argv[2]);

            if (x == null || y == null || z == null)
            {
                AnsiConsole.MarkupLine($"[red]Error: Coordinates invalid[/]");
                return;
            }


            var goal = new GoalXYZ(x.Value, y.Value, z.Value);
            var pathfinder = new Pathfinder(BotClient.Bot!);
            await pathfinder.Initialize();
            await AnsiConsole.Status()
                .StartAsync("Pathfinding...", async ctx => {
                    await pathfinder.GoTo(goal);
                });
        }
    }
}
