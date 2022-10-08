using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using MineSharp.Core.Types;
using Spectre.Console;

namespace MineSharp.ConsoleClient.Console.Commands.World
{
    internal class GetBlockAtCommand : Command
    {

        private IntegerArgument X = new IntegerArgument("x");
        private IntegerArgument Y = new IntegerArgument("y");
        private IntegerArgument Z = new IntegerArgument("z");

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
                AnsiConsole.MarkupLine($"[red]Error: Coordinates invalid[/]");
                return;
            }

            var block = BotClient.Bot!.GetBlockAt(new Position((int)x, (int)y, (int)z));
            var biome = BotClient.Bot!.GetBiomeAt(new Position((int)x, (int)y, (int)z));
            var table = new Table().AddColumns("Name", "Position", "Metadata", "Properties", "Biome");

            string propGetValue(BlockStateProperty prop)
            {
                switch (prop.Type)
                {
                    case BlockStateProperty.BlockStatePropertyType.Bool:
                        return prop.GetValue<bool>().ToString();
                    case BlockStateProperty.BlockStatePropertyType.Int:
                        return prop.GetValue<int>().ToString();
                    case BlockStateProperty.BlockStatePropertyType.Enum:
                        return prop.GetValue<string>();
                    default:
                        throw new NotSupportedException();
                }
            }

            var properties = string.Join("\n", block.Properties.Properties.Select(x => $"{x.Name}: {propGetValue(x)}"));

            table.AddRow(block.Name, block.Position!.ToString(), block.Metadata.ToString(), properties, biome.Name);
            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();
        }
    }
}
