using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using MineSharp.Data.T4.Blocks;
using Spectre.Console;

namespace MineSharp.ConsoleClient.Console.Commands.World {
    internal class MineBlockAtCommand : Command {

        IntegerArgument X = new IntegerArgument("x");
        IntegerArgument Y = new IntegerArgument("y");
        IntegerArgument Z = new IntegerArgument("z");

        public MineBlockAtCommand() {

            this.Initialize("mineBlockAt", 
                $"Mines a [purple]block[/] at the [{X.Color}]x y z[/] coordinates", 
                CColor.WorldCommand, X, Y, Z);
        }

        public override void DoAction(string[] argv, CancellationToken cancellation) {
            int? x = X.GetValue(argv[0]);
            int? y = Y.GetValue(argv[1]);
            int? z = Z.GetValue(argv[2]);

            if (x == null || y == null || z == null) {
                AnsiConsole.MarkupLine($"[red]Error: Coordinates invalid");
                return;
            }

            var block = BotClient.Bot.World.GetBlockAt(new Core.Types.Position((int)x, (int)y, (int)z));
            var breakingTime = block.CalculateBreakingTime(BotClient.Bot.HeldItem, BotClient.Bot.BotEntity!);

            AnsiConsole.Progress()
                .Start(ctx => {
                    AnsiConsole.MarkupLine("[green]Mining " + block + "[/]");

                    var pg = ctx.AddTask("Mining progress: ");

                    var startTime = DateTime.Now;
                    var task = BotClient.Bot.MineBlock(block, cancellation: cancellation);

                    while (DateTime.Now - startTime < TimeSpan.FromMilliseconds(breakingTime)) {
                        if (cancellation.IsCancellationRequested) {
                            return;
                        }
                        if (task.IsCompleted) {
                            break;
                        }
                        var percentage = ((DateTime.Now - startTime).TotalMilliseconds / breakingTime) * 100;
                        pg.Increment(percentage - pg.Value);
                        Task.Delay(100).Wait();
                    }
                    pg.Increment(pg.MaxValue - pg.Value);


                    var status = task.GetAwaiter().GetResult();

                    if (status == Bot.Enums.MineBlockStatus.Finished) {

                        AnsiConsole.MarkupLine("[green]Successfully mined block![/]");
                    } else {
                        AnsiConsole.MarkupLine("[red]Could not mine block: " + Enum.GetName(typeof(Bot.Enums.MineBlockStatus), status) + "[/]");
                    }

                });
        }
    }
}
