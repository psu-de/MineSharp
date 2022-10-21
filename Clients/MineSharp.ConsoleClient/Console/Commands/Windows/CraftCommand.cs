using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using MineSharp.ConsoleClient.Console.Commands.Arguments.Specialized;
using MineSharp.Core.Types;
using MineSharp.Data.Blocks;
using MineSharp.Data.Items;
using Spectre.Console;

namespace MineSharp.ConsoleClient.Console.Commands.Windows
{
    internal class CraftCommand : Command
    {
        private EnumArgument<ItemType> itemArg = new EnumArgument<ItemType>("itemType");
        private IntegerArgument X = new IntegerArgument("x", true);
        private IntegerArgument Y = new IntegerArgument("y", true);
        private IntegerArgument Z = new IntegerArgument("z", true);

        public CraftCommand()
        {
            var desc = $"Crafts an item of type [{this.itemArg.Color}]itemArg[/] (with the craftingTable at [{this.X.Color}]x y z[/] when needed)";
            this.Initialize("craft", desc, CColor.WindowsCommand, this.itemArg, this.X, this.Y, Z);
        }
        
        public override void DoAction(string[] argv, CancellationToken cancellation)
        {
            var itemType = this.itemArg.GetValue(argv[0]);

            CraftingTable? table = null;
            
            if (argv.Length == 4)
            {
                var x = this.X.GetValue(argv[1]);
                var y = this.X.GetValue(argv[2]);
                var z = this.X.GetValue(argv[3]);
                var block = Client.BotClient.Bot!.GetBlockAt(new Position(x!.Value, y!.Value, z!.Value));

                if (block.Id != CraftingTable.BlockId)
                {
                    AnsiConsole.MarkupLine($"[{CColor.Error}]Block is not a crafting table[/]");
                    return;
                }
                table = (CraftingTable)block;   
            }

            var recipe = BotClient.Bot!.FindRecipe(itemType);
            if (recipe == null)
            {
                AnsiConsole.MarkupLine($"[{CColor.Error}]No recipe found[/]");
                return;
            }

            var atable = new Table();
            if (recipe.RequiresCraftingTable)
            {
                atable.AddColumns("1", "2", "3");
                for (int i = 0; i < 3; i++)
                {
                    var i1 = (recipe.Ingredients.Length > (i * 3)) ? recipe.Ingredients[i * 3] : null;
                    var i2 = (recipe.Ingredients.Length > (i * 3) + 1) ? recipe.Ingredients[i * 3 + 1] : null;
                    var i3 = (recipe.Ingredients.Length > (i * 3) + 2) ? recipe.Ingredients[i * 3 + 2] : null;

                    atable.AddRow(new Panel(i1?.ToString() ?? ""), new Panel(i2?.ToString() ?? ""), new Panel(i3?.ToString() ?? ""));
                }
            }
            AnsiConsole.Write(atable);
            BotClient.Bot!.Craft(recipe, table, 1).Wait(cancellation);
        }
    }
}
