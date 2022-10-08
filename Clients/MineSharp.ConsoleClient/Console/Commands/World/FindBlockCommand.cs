using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using MineSharp.Data.Blocks;
using Spectre.Console;
namespace MineSharp.ConsoleClient.Console.Commands.World
{
    internal class FindBlockCommand : Command
    {

        private readonly EnumArgument<BlockType> BlockTypeArgument;

        public FindBlockCommand()
        {

            this.BlockTypeArgument = new EnumArgument<BlockType>("blockType");

            var desc = $"Searches for an Block of the specified [{this.BlockTypeArgument.Color}]Block Type[/]";

            this.Initialize("findBlock", desc, CColor.WorldCommand, this.BlockTypeArgument);
        }

        public override void DoAction(string[] argv, CancellationToken cancellation)
        {
            AnsiConsole.Status()
                .Start("Searching for [yellow]" + argv[0] + "[/]", ctx =>
                {
                    var blockType = this.BlockTypeArgument.GetValue(argv[0]);


                    var block = BotClient.Bot!.FindBlockAsync(blockType, cancellation).GetAwaiter().GetResult();
                    if (cancellation.IsCancellationRequested) return;

                    if (block != null)
                    {
                        AnsiConsole.MarkupLine("[green] Found Block: " + block + "[/]");
                    } else
                    {
                        AnsiConsole.MarkupLine("[darkorange] No block was found![/]");
                    }

                });
        }
    }
}
