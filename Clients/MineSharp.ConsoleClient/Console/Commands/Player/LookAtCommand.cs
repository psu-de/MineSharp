using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using MineSharp.Core.Types;
using Spectre.Console;

namespace MineSharp.ConsoleClient.Console.Commands.Player
{
    internal class LookAtCommand : Command
    {

        private readonly IntegerArgument X;
        private readonly IntegerArgument Y;
        private readonly IntegerArgument Z;

        public LookAtCommand()
        {
            this.X = new IntegerArgument("x");
            this.Y = new IntegerArgument("y");
            this.Z = new IntegerArgument("y");

            var desc = $"Makes the Player look at the specified coordinates [{this.X.Color}]X Y Z[/]";
            this.Initialize("lookAt", desc, CColor.PlayerCommand, this.X, this.Y, this.Z);
        }

        public override void DoAction(string[] argv, CancellationToken cancellation)
        {

            var x = this.X.GetValue(argv[0]);
            var y = this.Y.GetValue(argv[1]);
            var z = this.Y.GetValue(argv[2]);

            if (x == null || y == null || z == null)
            {
                AnsiConsole.MarkupLine($"[{CColor.Error}]Invalid pitch![/]");
            }

            BotClient.Bot!.ForceLookAt(new Position(x!.Value, y!.Value, z!.Value));
        }
    }
}
