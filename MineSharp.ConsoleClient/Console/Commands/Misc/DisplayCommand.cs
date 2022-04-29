using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.ConsoleClient.Console.Commands.Misc {
    internal class DisplayCommand : Command {

        public EnumArgument<DisplayOption> OptionArgument;

        public DisplayCommand() {
            OptionArgument = new EnumArgument<DisplayOption>("option");

            var desc = $"Can display information about several [{OptionArgument.Color}]options[/]";

            this.Initialize("display", desc, CColor.MiscCommand, OptionArgument);
        }

        public override void PrintHelp() {
            base.PrintHelp();

            var tbl = new Table().AddColumn("Possible Options");
            foreach (var opt in Enum.GetNames(typeof(DisplayOption))) {
                tbl.AddRow(opt);
            }
            AnsiConsole.Write(tbl);
        }

        public override void DoAction(string[] argv, CancellationToken cancellation) {

            var option = OptionArgument.GetValue(argv[0]);

            switch (option) {
                case DisplayOption.Players: WritePlayerTable(); break;
                case DisplayOption.Inventory: WriteInventory(); break;
                case DisplayOption.BotLog: WriteBotLog(); break;
                case DisplayOption.PlayerInfo: WritePlayerInfo(); break;
            }

        }

        void WritePlayerTable() {
            var output = new Table()
                        .AddColumns("Player Name", "Position", "ID", "UUID");
            foreach (var player in BotClient.Bot.PlayerList) {
                output.AddRow(player.Username, player.Position.ToString(), player.Id.ToString(), player.UUID.ToString());
            }
            AnsiConsole.Write(output);
        }

        void WriteInventory() {

            var inventory = new Table()
                .AddColumns("Slot Id", "Item Name");

            foreach (var slot in BotClient.Bot.Inventory.Slots.Take(BotClient.Bot.Inventory.HotbarStart)) {
                inventory.AddRow(slot.Key.ToString(), slot.Value.Item?.Info.DisplayName ?? "");
            }
            AnsiConsole.Write(inventory);

            var hotbar = new Table();
            hotbar.AddColumns("1", "2", "3", "4", "5", "6", "7", "8", "9");
            foreach (var col in hotbar.Columns) {
                col.Width(AnsiConsole.Profile.Width / 9).Centered();
            }

            var hotbarSlots = BotClient.Bot.Inventory.GetHotbarSlots();
            hotbar.AddRow(hotbarSlots.Select(x => new Text(x.Item?.Info.DisplayName ?? "").Centered()).ToArray());

            AnsiConsole.MarkupLine("[olive]Hotbar: [/]");
            AnsiConsole.Write(hotbar);
        }

        void WriteBotLog() {
            var stream = new MemoryStream(BotClient.BotLog.ToArray());
            var reader = new StreamReader(stream);

            var fullLog = reader.ReadToEnd();
            AnsiConsole.WriteLine(fullLog);
        }

        void WritePlayerInfo() {
            AnsiConsole.Write(new BarChart()
                .Width(30)
                .Label("[green underline]Stats[/]")
                .CenterLabel()
                .AddItem("Health", BotClient.Bot.Health, Spectre.Console.Color.Maroon)
                .AddItem("Food", BotClient.Bot.Food, Spectre.Console.Color.Orange4_1)
                .AddItem("Saturation", BotClient.Bot.Saturation, Spectre.Console.Color.Yellow4));

            AnsiConsole.Write(new Table().AddColumns("Position", "OnGround").AddRow(BotClient.Bot.BotEntity.Position.ToString(), BotClient.Bot.BotEntity.IsOnGround.ToString()));
            AnsiConsole.WriteLine();
        }

        internal enum DisplayOption {
            Players,
            PlayerInfo,
            Inventory,
            BotLog
        }
    }
}
