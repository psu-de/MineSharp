using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using MineSharp.Core.Types;
using Spectre.Console;

namespace MineSharp.ConsoleClient.Console.Commands.Windows
{
    internal class OpenContainerCommand : Command
    {

        private readonly IntegerArgument X = new IntegerArgument("x");
        private readonly IntegerArgument Y = new IntegerArgument("y");
        private readonly IntegerArgument Z = new IntegerArgument("z");

        public OpenContainerCommand()
        {
            var desc = "Opens a block at a given position";
            this.Initialize("openContainer", desc, CColor.WindowsCommand, this.X, this.Y, this.Z);
        }

        public override void DoAction(string[] argv, CancellationToken cancellation)
        {

            throw new NotImplementedException();
            // var x = (int)this.X.GetValue(argv[0])!;
            // var y = (int)this.Y.GetValue(argv[1])!;
            // var z = (int)this.Z.GetValue(argv[2])!;
            //
            // AnsiConsole.MarkupLine($"[green] Opening container at ({x} / {y} / {z}) [/]");
            //
            // var block = BotClient.Bot!.GetBlockAt(new Position(x, y, z));
            // var window = BotClient.Bot.OpenContainer(block).GetAwaiter().GetResult();
            //
            // AnsiConsole.MarkupLine($"[green]Opened window with id={window.Id}[/]");
        }
    }
}
