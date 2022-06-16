using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using MineSharp.Data.Blocks;
using Spectre.Console;

namespace MineSharp.ConsoleClient.Console.Commands.World {
    internal class FindBlocksCommand : Command {

        EnumArgument<BlockType> BlockTypeArgument;
        IntegerArgument CountArgument;

        public FindBlocksCommand() {

            BlockTypeArgument = new EnumArgument<BlockType>("blockType");
            CountArgument = new IntegerArgument("count", true);
            var desc = $"Searches for [{CountArgument.Color}]count (default Infinity)[/] Blocks of the specified [{BlockTypeArgument.Color}]Block Type[/]";

            this.Initialize("findBlocks", 
                desc,
                CColor.WorldCommand, BlockTypeArgument, CountArgument);
        }

        public override void PrintHelp() {
            base.PrintHelp();
            AnsiConsole.MarkupLine($"Info: Use [{CountArgument.Color}]count < 0[/] to search without a limit");
        }

        public override void DoAction(string[] argv, CancellationToken cancellation) {

            int count = -1;
            if (argv.Length > 1) {
                var c = CountArgument.GetValue(argv[1]);

                if (c == null) {
                    AnsiConsole.MarkupLine("[red]Error: Invalid count[/]");
                    return;
                }
                count = c.Value;
            }

            var table = new Table();
            int? blockCount = null;

            AnsiConsole.Status()
                .Start($"Searching for [lightgreen]{(count < 0 ? new string("Infinity") : count.ToString())}[/] [yellow]{argv[0]}[/]", async ctx => {
                    var blockType = BlockTypeArgument.GetValue(argv[0]);
                    var block = BotClient.Bot.FindBlocksAsync(blockType, (int)count, cancellation).GetAwaiter().GetResult();
                    if (cancellation.IsCancellationRequested) return;

                    if (block != null) {
                        blockCount = block.Length;
                        if (blockCount > 100 && count < 0) {
                            block = block.Take(100).ToArray();
                            AnsiConsole.MarkupLine($"[{CColor.Good}] Found over 100 blocks, only displaying the first 100[/]");
                        }
                        table.AddColumns("Block Type", "Position");

                        foreach (var b in block) {
                            table.AddRow(b.Name, b.Position!.ToString());
                        }
                        block = null;
                    } else {
                        AnsiConsole.MarkupLine($"[{CColor.Warn}] No block was found![/]");
                    }

                });

            if (blockCount != null) {
                AnsiConsole.Write(table);
                AnsiConsole.MarkupLine($"[{CColor.Good}] Found " + blockCount + " blocks[/]");
            }
        }
    }
}
