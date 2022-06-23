using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using MineSharp.Data.Entities;
using Spectre.Console;

namespace MineSharp.ConsoleClient.Console.Commands.Entity {
    internal class GetEntitiesCommand : Command {

        IntegerArgument RangeArg;
        EnumArgument<EntityType> TypeArg;

        public GetEntitiesCommand() {

            RangeArg = new IntegerArgument("range", true);
            TypeArg = new EnumArgument<EntityType>("entityType", true);
            var desc = $"Prints a list of entities (optionally in a given [{RangeArg.Color}]range (default Infinity)[/] and/or of a specific [{TypeArg.Color}]Entity Type[/]";

            this.Initialize("getEntities", desc, CColor.EntityCommand, RangeArg, TypeArg);
        }

        public override void PrintHelp() {
            base.PrintHelp();

            AnsiConsole.MarkupLine($"Use [{RangeArg.Color}]range -1[/] for infinity range");
        }

        public override void DoAction(string[] argv, CancellationToken cancellation) {

            int range = -1;
            EntityType? type = null;
            if (argv.Length > 0) {
                int? r = RangeArg.GetValue(argv[0]);
                if (r == null) {
                    AnsiConsole.MarkupLine("[red]Invalid range[/]");
                    return;
                }

                if (argv.Length > 1) {
                    type = TypeArg.GetValue(argv[1]);
                }
                range = r.Value;
            }

            IEnumerable<Core.Types.Entity> entities = BotClient.Bot!.Entities.Values;
            if (range != -1) {
                entities = entities.Where(x => x.Position.Distance(BotClient.Bot.BotEntity!.Position) < range).ToList();
            }
            if (type != null) {
                entities = entities.Where(x => x.Id == (int)type);
            }

            Table table = new Table();

            table.AddColumns("Entity Id", "Type", "Position");
            foreach (var e in entities) {
                table.AddRow(e.Id.ToString(), e.Name, e.Position.ToString());
            }

            if (table.Rows.Count > 0) {
                AnsiConsole.Write(table);
                AnsiConsole.MarkupLine($"[green]Found {entities.ToArray().Length} entities![/]");
            } else {
                AnsiConsole.MarkupLine($"[red]Nothing found![/]");
            }
        }
    }
}
