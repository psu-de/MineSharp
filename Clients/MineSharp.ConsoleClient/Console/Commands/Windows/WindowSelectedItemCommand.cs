using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using MineSharp.ConsoleClient.Console.Commands.Arguments.Specialized;
using PrettyPrompt.Completion;
using Spectre.Console;

namespace MineSharp.ConsoleClient.Console.Commands.Windows {
    internal class WindowSelectedItemCommand : Command {

        public CustomAutocompleteArgument WindowIDArgument;

        public WindowSelectedItemCommand()
        {
            this.WindowIDArgument = new CustomAutocompleteArgument(new IntegerArgument("windowId"), GetWindowIdItems);

            var desc = $"Displays the selected item of a given window";
            this.Initialize("windowSelectedItem", desc, CColor.WindowsCommand, WindowIDArgument);
        }

        public override void DoAction(string[] argv, CancellationToken cancellation)
        {
            var windowId = new IntegerArgument("").GetValue(argv[0]);
            if (windowId == null)
                throw new ArgumentNullException(nameof(windowId));

            if (!BotClient.Bot!.OpenedWindows.TryGetValue(windowId.Value, out var window))
            {
                throw new Exception("Window with id " + windowId + " not found!");
            }

            AnsiConsole.MarkupLine($"Selected window item: " + window.SelectedSlot?.Item ?? "null");
        }

        private List<CompletionItem> GetWindowIdItems(string arg) {
            return BotClient.Bot!.OpenedWindows.Select(x => {
                return new CompletionItem(
                    replacementText: x.Key.ToString(),
                    displayText: CColor.FromMarkup($"[darkorange3_1]{x.Key} ({x.Value.Info.Name})[/]"));
            }).ToList();
        }
    }
}
