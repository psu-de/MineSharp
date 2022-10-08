using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using MineSharp.Core;
using Spectre.Console;
namespace MineSharp.ConsoleClient.Console.Commands.Misc
{
    internal class DisplayCommand : Command
    {

        public EnumArgument<DisplayOption> OptionArgument;

        public DisplayCommand()
        {
            this.OptionArgument = new EnumArgument<DisplayOption>("option");

            var desc = $"Can display information about several [{this.OptionArgument.Color}]options[/]";

            this.Initialize("display", desc, CColor.MiscCommand, this.OptionArgument);
        }

        public override void PrintHelp()
        {
            base.PrintHelp();

            var tbl = new Table().AddColumn("Possible Options");
            foreach (var opt in Enum.GetNames(typeof(DisplayOption)))
            {
                tbl.AddRow(opt);
            }
            AnsiConsole.Write(tbl);
        }

        public override void DoAction(string[] argv, CancellationToken cancellation)
        {

            var option = this.OptionArgument.GetValue(argv[0]);

            switch (option)
            {
                case DisplayOption.Players:
                    this.WritePlayerTable();
                    break;
                case DisplayOption.Inventory:
                    this.WriteInventory();
                    break;
                case DisplayOption.PlayerInfo:
                    this.WritePlayerInfo();
                    break;
            }

        }

        private void WritePlayerTable()
        {
            var output = new Table()
                .AddColumns("Player Name", "Position", "ID", "UUID");
            foreach (var player in BotClient.Bot!.PlayerList)
            {
                output.AddRow(player.Username, player.Entity.Position.ToString(), player.Entity.ServerId.ToString(), player.UUID.ToString());
            }
            AnsiConsole.Write(output);
        }

        private void WriteInventory()
        {

            var inventory = new Table()
                .AddColumns("Slot Id", "Item Name", "Count");

            if (BotClient.Bot!.Inventory == null)
            {
                AnsiConsole.MarkupLine("[red] Inventory not loaded yet.[/]");
                return;
            }

            foreach (var slot in BotClient.Bot.Inventory!.GetAllSlots())
            {
                inventory.AddRow(slot.SlotNumber!.ToString(), slot.Item?.DisplayName ?? "", slot.Item?.Count.ToString() ?? "");
            }
            AnsiConsole.Write(inventory);
        }

        private void WritePlayerInfo()
        {

            if (BotClient.Bot!.BotEntity == null)
            {
                AnsiConsole.MarkupLine("[red]Player is not loaded yet[/]");
                return;
            }

            var masterTable = new Table().AddColumns("Key", "Data");
            masterTable.Border(TableBorder.Rounded);

            var statsInfo = new Table().AddColumns(new TableColumn("Key"), new TableColumn("Value").Width(26));
            string getStatRow(int val)
            {
                var block = '█';
                return (string.Join("", Enumerable.Repeat(block, val)) + $" ({val.ToString().PadLeft(2, '0')})").TrimStart();
            }
            statsInfo.AddRow(new Text("Health"), new Markup($"[maroon]{getStatRow((int)BotClient.Bot.Health)}[/]"));
            statsInfo.AddRow(new Text("Food"), new Markup($"[orange4_1]{getStatRow((int)BotClient.Bot.Food)}[/]"));
            statsInfo.AddRow(new Text("Saturation"), new Markup($"[yellow4]{getStatRow((int)BotClient.Bot.Saturation)}[/]"));

            masterTable.AddRow(new Markup("\n[green underline]Stats[/]"), statsInfo);

            var effectsInfo = new Table().AddColumns("Effect Name", "Level", "Duration");
            foreach (var effect in BotClient.Bot.BotEntity.Effects.Values)
            {
                if (effect == null) continue;
                var eColor = effect.IsGood ? "springgreen2" : "red1";
                effectsInfo.AddRow($"[{eColor}]{effect.DisplayName}[/]", (effect.Amplifier + 1).ToString(), (effect.Duration * MinecraftConst.TickMs / 1000) + "s");
            }

            masterTable.AddRow(new Markup("\n[green underline]Effects[/]"), effectsInfo);

            var positionInfo = new Table().AddColumns("Position", "Velocity", "OnGround", "Yaw", "Pitch")
                .AddRow(BotClient.Bot.BotEntity.Position.ToString(),
                    BotClient.Bot.BotEntity.Velocity.ToString(),
                    BotClient.Bot.BotEntity.IsOnGround.ToString(),
                    BotClient.Bot.BotEntity.Yaw.ToString(),
                    BotClient.Bot.BotEntity.Pitch.ToString());
            masterTable.AddRow(new Markup("\n[green underline]Position[/]"), positionInfo);


            masterTable.AddRow(new Markup("\n[green underline]Held Item[/]"), new Panel(new Text(BotClient.Bot.HeldItem?.ToString() ?? "No Item")));

            AnsiConsole.Write(masterTable);
        }

        internal enum DisplayOption
        {
            Players,
            PlayerInfo,
            Inventory
        }
    }
}
