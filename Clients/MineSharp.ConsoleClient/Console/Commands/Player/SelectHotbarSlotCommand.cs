using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using Spectre.Console;

namespace MineSharp.ConsoleClient.Console.Commands.Player
{
    internal class SelectHotbarSlotCommand : Command
    {

        private IntegerArgument SlotId = new IntegerArgument("slotNumber");

        public SelectHotbarSlotCommand()
        {
            var desc = $"Switches the selected hotbar slot";

            this.Initialize("selectHotbarSlot", desc, CColor.PlayerCommand, this.SlotId);
        }

        public override void DoAction(string[] argv, CancellationToken cancellation)
        {

            var slot = this.SlotId.GetValue(argv[0]);
            if (slot == null || slot < 1 || slot > 9)
            {
                AnsiConsole.MarkupLine("[red]Invalid Slot number, must be between 1 and 9[/]");
                return;
            }

            BotClient.Bot!.SelectHotbarIndex((byte)slot);
        }
    }
}
