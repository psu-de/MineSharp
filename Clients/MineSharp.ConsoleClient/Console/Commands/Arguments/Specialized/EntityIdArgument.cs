using MineSharp.ConsoleClient.Client;
using PrettyPrompt.Completion;

namespace MineSharp.ConsoleClient.Console.Commands.Arguments.Specialized {
    internal class EntityIdArgument : IntegerArgument {
        public EntityIdArgument(string name, bool isOptional = false) : base(name, isOptional) {
        }

        public override List<CompletionItem> GetCompletionItems(string str) {
           
            List<CompletionItem> items = new List<CompletionItem>();

            foreach (var e in BotClient.Bot!.Entities.Values) {

                string displayText = $"[{Color}]{e.ServerId} ({e.Name})[/]";

                items.Add(
                    new CompletionItem(
                        replacementText: e.ServerId.ToString(),
                        displayText: CColor.FromMarkup(displayText)));
            }
            return items;
        }
    }
}
