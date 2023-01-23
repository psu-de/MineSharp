using MineSharp.ConsoleClient.Client;
using PrettyPrompt.Completion;

namespace MineSharp.ConsoleClient.Console.Commands.Arguments.Specialized
{
    internal class EntityIdArgument : IntegerArgument
    {
        public EntityIdArgument(string name, bool isOptional = false) : base(name, isOptional) {}

        public override List<CompletionItem> GetCompletionItems(string str)
        {

            var items = new List<CompletionItem>();

            foreach (var e in BotClient.Bot!.Entities.Values)
            {

                var displayText = $"[{this.Color}]{e.ServerId} ({e.Info.Name})[/]";

                items.Add(
                    new CompletionItem(
                        e.ServerId.ToString(),
                        CColor.FromMarkup(displayText)));
            }
            return items;
        }
    }
}
