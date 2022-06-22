using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using MineSharp.ConsoleClient.Console.Commands.Arguments.Specialized;
using MineSharp.Core.Types.Enums;
using MineSharp.Windows;
using PrettyPrompt.Completion;
using Spectre.Console;

namespace MineSharp.ConsoleClient.Console.Commands.Windows {
    internal class ClickWindowCommand : Command {

        public CustomAutocompleteArgument WindowIDArgument;
        public EnumArgument<WindowOperationMode> WindowOperationModeArgument;
        public IntegerArgument ButtonArgument;
        public IntegerArgument SlotArgument;

        public ClickWindowCommand() {
            this.WindowIDArgument = new CustomAutocompleteArgument(new IntegerArgument("windowId"), GetWindowIdItems);
            this.WindowOperationModeArgument = new EnumArgument<WindowOperationMode>("clickMode");
            this.ButtonArgument = new IntegerArgument("button");
            this.SlotArgument = new IntegerArgument("slot");


            var desc = $"Performs a [yellow]click[/] on a given windowId";
            this.Initialize("clickWindow", desc, CColor.WindowsCommand, WindowIDArgument, WindowOperationModeArgument, ButtonArgument, SlotArgument);
        }

        public override void PrintHelp() {
            base.PrintHelp();

            AnsiConsole.MarkupLine("button=0 > Left Click");
            AnsiConsole.MarkupLine("button=1 > Right Click");
            AnsiConsole.MarkupLine("button=2 > Middle Click");

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("Use slot = -999 to perform a click outside the window");
        }

        private List<CompletionItem> GetWindowIdItems(string arg) {
            return BotClient.Bot.OpenedWindows.Select(x => {
                return new CompletionItem(
                    replacementText: x.Key.ToString(),
                    displayText: CColor.FromMarkup($"[darkorange3_1]{x.Key} ({x.Value.Info.Name})[/]"));
            }).ToList();
        }

        public override void DoAction(string[] argv, CancellationToken cancellation) {

            var windowId = ButtonArgument.GetValue(argv[0]);
            var clickMode = WindowOperationModeArgument.GetValue(argv[1]);
            var button = ButtonArgument.GetValue(argv[2]);
            var slot = SlotArgument.GetValue(argv[3]);

            if (windowId == null) throw new ArgumentException("windowId");
            if (button == null || button >= byte.MaxValue) throw new ArgumentException("button");
            if (slot == null || button >= short.MaxValue) throw new ArgumentException("slot");

            WindowClick click = new WindowClick(clickMode, (byte)button, (short)slot);
            BotClient.Bot.OpenedWindows.TryGetValue((int)windowId, out var window);

            if (window == null) throw new ArgumentException($"Window with id={windowId} not opened");

            window.PerformClick(click);
        }
    }
}
