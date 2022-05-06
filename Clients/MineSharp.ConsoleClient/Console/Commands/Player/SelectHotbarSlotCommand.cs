using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.ConsoleClient.Console.Commands.Player {
    internal class SelectHotbarSlotCommand : Command {

        IntegerArgument SlotId = new IntegerArgument("slotNumber");

        public SelectHotbarSlotCommand() {
            var desc = $"Switches the selected hotbar slot";

            this.Initialize("selectHotbarSlot", desc, CColor.PlayerCommand, SlotId);
        }

        public override void DoAction(string[] argv, CancellationToken cancellation) {
            
            var slot = SlotId.GetValue(argv[0]);
            if (slot == null || slot < 1 || slot > 9) {
                AnsiConsole.MarkupLine("[red]Invalid Slot number, must be between 1 and 9[/]");
                return;
            }

            BotClient.Bot.SelectHotbarSlot((byte)slot);
        }
    }
}
