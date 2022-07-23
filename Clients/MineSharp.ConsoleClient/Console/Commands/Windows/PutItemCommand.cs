using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using MineSharp.ConsoleClient.Console.Commands.Arguments.Specialized;
using MineSharp.Core.Types.Enums;
using MineSharp.Data.Items;
using MineSharp.Windows;
using PrettyPrompt.Completion;
using Spectre.Console;

namespace MineSharp.ConsoleClient.Console.Commands.Windows {
    internal class PutItemCommand : Command {

        public CustomAutocompleteArgument WindowIDArgument;
        public EnumArgument<ItemType> ItemTypeArgument;
        public IntegerArgument CountArgument;

        public PutItemCommand() {
            this.WindowIDArgument = new CustomAutocompleteArgument(new IntegerArgument("windowId"), GetWindowIdItems);
            this.ItemTypeArgument = new EnumArgument<ItemType>("itemType");
            this.CountArgument = new IntegerArgument("count", true);

            var desc = $"Puts [{CountArgument.Color}]count[/] items of type [{ItemTypeArgument.Color}]itemType[/] from the bot inventory and places them in the given window.";
            this.Initialize("putItem", desc, CColor.WindowsCommand, WindowIDArgument, ItemTypeArgument, CountArgument);
        }

        private List<CompletionItem> GetWindowIdItems(string arg) {
            return BotClient.Bot!.OpenedWindows.Select(x => {
                return new CompletionItem(
                    replacementText: x.Key.ToString(),
                    displayText: CColor.FromMarkup($"[darkorange3_1]{x.Key} ({x.Value.Info.Name})[/]"));
            }).ToList();
        }

        public override void DoAction(string[] argv, CancellationToken cancellation) {

            var windowId = CountArgument.GetValue(argv[0]);
            var itemType = ItemTypeArgument.GetValue(argv[1]);
            byte? count = null;
            if (argv.Length > 2)
                count = (byte?)CountArgument.GetValue(argv[2]);

            if (windowId == null) throw new ArgumentException("windowId");

            BotClient.Bot!.OpenedWindows.TryGetValue((int)windowId, out var window);
            if (window == null) throw new ArgumentException($"Window with id={windowId} not opened");

            window.PutItem((int)itemType, count, null).Wait(cancellation);
        }
    }
}
