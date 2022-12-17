using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using MineSharp.ConsoleClient.Console.Commands.Arguments.Specialized;
using PrettyPrompt.Completion;
using Spectre.Console;

namespace MineSharp.ConsoleClient.Console.Commands.Windows
{
    internal class MoveItemsCommand : Command
    {
        public CustomAutocompleteArgument WindowIDArgument;
        public IntegerArgument SlotFromArgument;
        public IntegerArgument SlotToArgument;
        public IntegerArgument CountArgument;

        public MoveItemsCommand()
        {
            this.WindowIDArgument = new CustomAutocompleteArgument(new IntegerArgument("windowId"), this.GetWindowIdItems);
            this.SlotFromArgument = new IntegerArgument("slotFrom");
            this.SlotToArgument = new IntegerArgument("slotTo");
            this.CountArgument = new IntegerArgument("count");
            
            var desc = $"Moves [{this.CountArgument.Color}]items[/] from [{this.SlotFromArgument.Color}]slotFrom[/] to [{this.SlotFromArgument.Color}]slotTo[/]";
            this.Initialize("moveItems", desc, CColor.WindowsCommand, this.WindowIDArgument, this.SlotFromArgument, this.SlotToArgument, this.CountArgument);
        }

        private List<CompletionItem> GetWindowIdItems(string arg)
        {
            return BotClient.Bot!.OpenedWindows.Select(x =>
            {
                return new CompletionItem(
                    x.Key.ToString(),
                    CColor.FromMarkup($"[darkorange3_1]{x.Key} ({x.Value.Title})[/]"));
            }).ToList();
        }
        
        public override void PrintHelp()
        {
            base.PrintHelp();
        }

        public override void DoAction(string[] argv, CancellationToken cancellation)
        {
            int windowId = this.SlotFromArgument.GetValue(argv[0])!.Value;
            int slotFrom = this.SlotFromArgument.GetValue(argv[1])!.Value;
            int slotTo = this.SlotFromArgument.GetValue(argv[2])!.Value;
            int count = this.SlotFromArgument.GetValue(argv[3])!.Value;

            BotClient.Bot!.OpenedWindows.TryGetValue((int)windowId, out var window);

            if (window == null) throw new ArgumentException($"Window with id={windowId} not opened");
            
            window.MoveItemsFromSlot((short)slotFrom, (short)slotTo, count);
        }
    }
}
