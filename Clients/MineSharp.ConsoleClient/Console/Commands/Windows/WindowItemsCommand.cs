using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using MineSharp.ConsoleClient.Console.Commands.Arguments.Specialized;
using PrettyPrompt.Completion;
using Spectre.Console;

namespace MineSharp.ConsoleClient.Console.Commands.Windows {
    internal class WindowItemsCommand : Command {

        public CustomAutocompleteArgument WindowIDArgument;

        public WindowItemsCommand() {
            this.WindowIDArgument = new CustomAutocompleteArgument(new IntegerArgument("windowId"), GetWindowIdItems);
            var desc = $"Shows a table of all items in the given window";

            this.Initialize("windowItems", desc, CColor.WindowsCommand, WindowIDArgument);
        }

        private List<CompletionItem> GetWindowIdItems(string arg) {
            return BotClient.Bot!.OpenedWindows.Select(x => {
                return new CompletionItem(
                    replacementText: x.Key.ToString(),
                    displayText: CColor.FromMarkup($"[darkorange3_1]{x.Key} ({x.Value.Info.Name})[/]"));
            }).ToList();
        }

        public override void DoAction(string[] argv, CancellationToken cancellation) {
            var windowId = new IntegerArgument("").GetValue(argv[0]);

            if (!BotClient.Bot!.OpenedWindows.TryGetValue((int)windowId!, out var window)) {
                AnsiConsole.MarkupLine($"[red]Window with id={windowId} not opened![/]");
                return;
            }

            var windowTable = new Table()
                .AddColumns("Slot Id", "Item Name", "Count");

            if (BotClient.Bot.Inventory == null) {
                AnsiConsole.MarkupLine("[red] Inventory not loaded yet.[/]");
                return;
            }

            foreach (var slot in window.GetAllSlots()) {
                windowTable.AddRow(slot.SlotNumber!.ToString(), slot.Item?.DisplayName ?? "", slot.Item!.Count.ToString());
            }
            AnsiConsole.Write(windowTable);
        }
    }
}
