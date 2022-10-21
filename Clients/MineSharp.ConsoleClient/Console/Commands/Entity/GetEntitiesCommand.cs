using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using MineSharp.Data.Entities;
using Spectre.Console;

namespace MineSharp.ConsoleClient.Console.Commands.Entity
{
    internal class GetEntitiesCommand : Command
    {

        private readonly IntegerArgument RangeArg;
        private readonly EnumArgument<EntityType> TypeArg;

        public GetEntitiesCommand()
        {

            this.RangeArg = new IntegerArgument("range", true);
            this.TypeArg = new EnumArgument<EntityType>("entityType", true);
            var desc = $"Prints a list of entities (optionally in a given [{this.RangeArg.Color}]range (default Infinity)[/] and/or of a specific [{this.TypeArg.Color}]Entity Type[/]";

            this.Initialize("getEntities", desc, CColor.EntityCommand, this.RangeArg, this.TypeArg);
        }

        public override void PrintHelp()
        {
            base.PrintHelp();

            AnsiConsole.MarkupLine($"Use [{this.RangeArg.Color}]range -1[/] for infinity range");
        }

        public override void DoAction(string[] argv, CancellationToken cancellation)
        {

            var range = -1;
            EntityType? type = null;
            if (argv.Length > 0)
            {
                var r = this.RangeArg.GetValue(argv[0]);
                if (r == null)
                {
                    AnsiConsole.MarkupLine("[red]Invalid range[/]");
                    return;
                }

                if (argv.Length > 1)
                {
                    type = this.TypeArg.GetValue(argv[1]);
                }
                range = r.Value;
            }

            IEnumerable<Core.Types.Entity> entities = BotClient.Bot!.Entities.Values;
            if (range != -1)
            {
                entities = entities.Where(x => x.Position.Distance(BotClient.Bot.BotEntity!.Position) < range).ToList();
            }
            if (type != null)
            {
                entities = entities.Where(x => x.Id == (int)type);
            }

            var table = new Table();

            table.AddColumns("Entity Id", "Type", "Position");
            foreach (var e in entities)
            {
                table.AddRow(e.ServerId.ToString(), e.Name, e.Position.ToString());
            }

            if (table.Rows.Count > 0)
            {
                AnsiConsole.Write(table);
                AnsiConsole.MarkupLine($"[green]Found {entities.ToArray().Length} entities![/]");
            } else
            {
                AnsiConsole.MarkupLine("[red]Nothing found![/]");
            }
        }
    }
}
