using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using MineSharp.Core;
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

            throw new NotImplementedException();
            //var inventory = new Table()
            //    .AddColumns("Slot Id", "Item Name");

            //foreach (var slot in BotClient.Bot.Inventory.Slots.Take(BotClient.Bot.Inventory.HotbarStart)) {
            //    inventory.AddRow(slot.Key.ToString(), slot.Value.Item?.Info.DisplayName ?? "");
            //}
            //AnsiConsole.Write(inventory);

            //var hotbar = new Table();
            //hotbar.AddColumns("1", "2", "3", "4", "5", "6", "7", "8", "9");
            //foreach (var col in hotbar.Columns) {
            //    col.Width(AnsiConsole.Profile.Width / 9).Centered();
            //}

            //var hotbarSlots = BotClient.Bot.Inventory.GetHotbarSlots();
            //hotbar.AddRow(hotbarSlots.Select(x => new Text(x.Item?.ToString() ?? "").Centered()).ToArray());

            //AnsiConsole.MarkupLine("[olive]Hotbar: [/]");
            //AnsiConsole.Write(hotbar);
        }

        void WritePlayerInfo() {

            if (BotClient.Bot.BotEntity == null) {
                AnsiConsole.MarkupLine($"[red]Player is not loaded yet[/]");
                return;
            }

            var masterTable = new Table().AddColumns("Key", "Data");
            masterTable.Border(TableBorder.Rounded);

            var statsInfo = new Table().AddColumns(new TableColumn("Key"), new TableColumn("Value").Width(26));
            string getStatRow(int val) {
                char block = '█';
                return (string.Join("", Enumerable.Repeat(block, val)) + $" ({val.ToString().PadLeft(2, '0')})").TrimStart();
            }
            statsInfo.AddRow(new Text("Health"), new Markup($"[maroon]{getStatRow((int)BotClient.Bot.Health)}[/]"));
            statsInfo.AddRow(new Text("Food"), new Markup($"[orange4_1]{getStatRow((int)BotClient.Bot.Food)}[/]"));
            statsInfo.AddRow(new Text("Saturation"), new Markup($"[yellow4]{getStatRow((int)BotClient.Bot.Saturation)}[/]"));

            masterTable.AddRow(new Markup("\n[green underline]Stats[/]"), statsInfo);

            var effectsInfo = new Table().AddColumns("Effect Name", "Level", "Duration");
            foreach (var effect in BotClient.Bot.BotEntity.Effects.Values) {
                if (effect == null) continue;
                var eColor = effect.Info.IsGood ? "springgreen2" : "red1";
                effectsInfo.AddRow($"[{eColor}]{effect.Info.DisplayName}[/]", (effect.Amplifier + 1).ToString(), ((effect.Duration * MinecraftConst.TickMs) / 1000).ToString() + "s");
            }

            masterTable.AddRow(new Markup("\n[green underline]Effects[/]"), effectsInfo);

            var positionInfo = new Table().AddColumns("Position", "Velocity", "OnGround", "Yaw", "Pitch")
                .AddRow(BotClient.Bot.BotEntity.Position.ToString(),
                        BotClient.Bot.BotEntity.Velocity.ToString(),
                        BotClient.Bot.BotEntity.IsOnGround.ToString(),
                        BotClient.Bot.BotEntity.Yaw.ToString(),
                        BotClient.Bot.BotEntity.Pitch.ToString());
            masterTable.AddRow(new Markup("\n[green underline]Position[/]"), positionInfo);

            AnsiConsole.Write(masterTable);
        }

        internal enum DisplayOption {
            Players,
            PlayerInfo,
            Inventory
        }
    }
}
