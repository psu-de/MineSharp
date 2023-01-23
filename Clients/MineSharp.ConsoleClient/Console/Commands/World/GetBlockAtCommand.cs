using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using MineSharp.Core.Types;
using Spectre.Console;

namespace MineSharp.ConsoleClient.Console.Commands.World
{
    internal class GetBlockAtCommand : Command
    {

        private readonly IntegerArgument X = new IntegerArgument("x");
        private readonly IntegerArgument Y = new IntegerArgument("y");
        private readonly IntegerArgument Z = new IntegerArgument("z");

        public GetBlockAtCommand()
        {

            var desc = $"Gets a [purple]block[/] at the [{this.X.Color}]x y z[/] coordinates";

            this.Initialize("getBlockAt", desc, CColor.WorldCommand, this.X, this.Y, this.Z);
        }

        public override void DoAction(string[] argv, CancellationToken cancellation)
        {

            var x = this.X.GetValue(argv[0]);
            var y = this.Y.GetValue(argv[1]);
            var z = this.Z.GetValue(argv[2]);

            if (x == null || y == null || z == null)
            {
                AnsiConsole.MarkupLine("[red]Error: Coordinates invalid[/]");
                return;
            }

            var block = BotClient.Bot!.GetBlockAt(new Position((int)x, (int)y, (int)z));
            var biome = BotClient.Bot!.GetBiomeAt(new Position((int)x, (int)y, (int)z));
            var table = new Table().AddColumns("Name", "Position", "Metadata", "Properties", "Biome");

            string propGetValue(Block block, BlockStateProperty prop)
            {
                switch (prop.Type)
                {
                    case BlockStateProperty.BlockStatePropertyType.Bool:
                        return block.GetProperty<bool>(prop.Name).ToString();
                    case BlockStateProperty.BlockStatePropertyType.Int:
                        return block.GetProperty<int>(prop.Name).ToString();
                    case BlockStateProperty.BlockStatePropertyType.Enum:
                        return block.GetProperty<string>(prop.Name).ToString();
                    default:
                        throw new NotSupportedException();
                }
            }

            var properties = string.Join("\n", block.Info.Properties.Properties.Select(x => $"{x.Name}: {propGetValue(block, x)}"));

            table.AddRow(block.Info.Name, block.Position!.ToString(), block.Metadata.ToString(), properties, biome.Info.Name);
            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();
        }
    }
}
