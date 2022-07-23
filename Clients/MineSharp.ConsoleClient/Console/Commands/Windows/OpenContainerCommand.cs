using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using Spectre.Console;

namespace MineSharp.ConsoleClient.Console.Commands.Windows {
    internal class OpenContainerCommand : Command {

        IntegerArgument X = new IntegerArgument("x");
        IntegerArgument Y = new IntegerArgument("y");
        IntegerArgument Z = new IntegerArgument("z");

        public OpenContainerCommand() {
            var desc = $"Opens a block at a given position";
            this.Initialize("openContainer", desc, CColor.WindowsCommand, X, Y, Z);
        }

        public override void DoAction(string[] argv, CancellationToken cancellation) {

            int x = (int)X.GetValue(argv[0])!;
            int y = (int)Y.GetValue(argv[1])!;
            int z = (int)Z.GetValue(argv[2])!;

            AnsiConsole.MarkupLine($"[green] Opening container at ({x} / {y} / {z}) [/]");

            var block = BotClient.Bot!.GetBlockAt(new Core.Types.Position(x, y, z));
            var task = BotClient.Bot.OpenContainer(block);
            try {
                task.Wait(cancellation);

                var window = task.GetAwaiter().GetResult();

                AnsiConsole.MarkupLine($"[green]Opened window with id={window.Id}[/]");
            }
            catch (OperationCanceledException) {
            }
        }
    }
}
