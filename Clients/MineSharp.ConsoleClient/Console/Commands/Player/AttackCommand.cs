using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using MineSharp.ConsoleClient.Console.Commands.Arguments.Specialized;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.ConsoleClient.Console.Commands.Player {
    internal class AttackCommand : Command {

        EntityIdArgument EntityIdArg = new EntityIdArgument("entityId");
        public AttackCommand() {
            string desc = $"Attacks an [purple]Entity[/] specified by the [{EntityIdArg.Color}]Entity Id[/]";

            this.Initialize("attack", desc, CColor.PlayerCommand, EntityIdArg);
        }

        public override void DoAction(string[] argv, CancellationToken cancellation) {

            int? eId = EntityIdArg.GetValue(argv[0]);

            if (null == eId) {
                AnsiConsole.MarkupLine($"[red]Invalid entity id[/]");
                return;
            }

            if (!BotClient.Bot.Entities.ContainsKey(eId.Value)) {
                AnsiConsole.MarkupLine($"[red]Entity with id {eId.Value} does not exist[/]");
                return;
            }

            var entity = BotClient.Bot.Entities[eId.Value];
            BotClient.Bot.Attack(entity).GetAwaiter().GetResult();
            AnsiConsole.MarkupLine($"[green]Entity {eId.Value} attacked![/]");
        }
    }
}
