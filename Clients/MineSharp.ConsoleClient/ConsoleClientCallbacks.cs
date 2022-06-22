using MineSharp.ConsoleClient.Console;
using PrettyPrompt;
using PrettyPrompt.Completion;
using PrettyPrompt.Consoles;
using PrettyPrompt.Documents;
using PrettyPrompt.Highlighting;

namespace MineSharp.ConsoleClient {
    internal class ConsoleClientCallbacks : PromptCallbacks {

        protected override Task<bool> ShouldOpenCompletionWindowAsync(string text, int caret, KeyPress keyPress, CancellationToken cancellationToken) {
            return Task.FromResult(true);
        }

        protected override Task<IReadOnlyList<CompletionItem>> GetCompletionItemsAsync(string text, int caret, TextSpan spanToBeReplaced, CancellationToken cancellationToken) {

            if (!text.TrimStart().Contains(' ')) {
                // Command selection
                return Task.FromResult<IReadOnlyList<CompletionItem>>(CommandManager.Commands.Values.OrderBy(x => x.Color)
                    .Select(x => {

                    return new CompletionItem(
                        replacementText: x.Name,
                        displayText: new FormattedString(x.Name, new FormatSpan(0, x.Name.Length, CColor.GetAnsiColor(x.Color))),
                        getExtendedDescription: _ => Task.FromResult(CColor.FromMarkup(x.Description))
                        );
                }).ToArray());
            } else {

                string cmdName = text.TrimStart().Split(' ')[0];
                if (!CommandManager.TryGetCommand(cmdName, out var command)) {
                    return Task.FromResult<IReadOnlyList<CompletionItem>>(new List<CompletionItem>());
                }

                string args = text.Substring(cmdName.Length + (text.Length - text.TrimStart().Length)).TrimStart();
                (var currentArg, var remaining) = command.GetCurrentArgument(args);
                if (currentArg == null) {
                    return Task.FromResult<IReadOnlyList<CompletionItem>>(new List<CompletionItem>());
                }

                return Task.FromResult<IReadOnlyList<CompletionItem>>(currentArg.GetCompletionItems(remaining));
            }

        }

        protected override Task<IReadOnlyCollection<FormatSpan>> HighlightCallbackAsync(string text, CancellationToken cancellationToken) {

            string trimmed = text.TrimStart();
            string commandName = trimmed.Split(' ')[0];

            bool isInvalidCommandName = !CommandManager.TryGetCommand(commandName, out var command);

            List<FormatSpan> highlighting = new List<FormatSpan>();
            highlighting.Add(new FormatSpan(0, commandName.Length + (text.Length - trimmed.Length), isInvalidCommandName ? AnsiColor.Red : AnsiColor.Cyan));

            if (command != null) {
                string args = trimmed.Substring(commandName.Length);
                int offset = commandName.Length + (text.Length - trimmed.Length);
                offset += args.Length - args.TrimStart().Length;
                args = args.TrimStart();

                highlighting.AddRange(command.GetArgHighlighting(args, offset));
            }


            return Task.FromResult<IReadOnlyCollection<FormatSpan>>(highlighting);
        }


    }
}
