using MineSharp.ConsoleClient.Client;
using PrettyPrompt.Completion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.ConsoleClient.Console.Commands.Arguments.Specialized {
    internal class EntityIdArgument : IntegerArgument {
        public EntityIdArgument(string name, bool isOptional = false) : base(name, isOptional) {
        }

        public override List<CompletionItem> GetCompletionItems(string str) {
           
            List<CompletionItem> items = new List<CompletionItem>();

            foreach (var e in BotClient.Bot.EntitiesMapping.Values) {

                string displayText = $"[{Color}]{e.Id} ({e.EntityInfo.Name})[/]";

                items.Add(
                    new CompletionItem(
                        replacementText: e.Id.ToString(),
                        displayText: CColor.FromMarkup(displayText)));
            }
            return items;
        }
    }
}
