using PrettyPrompt.Completion;
using PrettyPrompt.Highlighting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.ConsoleClient.Console.Commands.Arguments {
    internal class CommandNameArgument : Argument {
        public CommandNameArgument(string name, bool isOptional = false) : base(name, isOptional) {
        }

        public override string Color { get => "purple"; set => throw new NotImplementedException(); }

        public override List<CompletionItem> GetCompletionItems(string str) {

            return CommandManager.Commands.Values.Select(x => {
                return new CompletionItem(
                    replacementText: x.Name,
                    displayText: new FormattedString(x.Name, new ConsoleFormat(CColor.GetAnsiColor(x.Color)))
                    );
            }).ToList();

        }

        public override bool IsValid(string str) {
            return CommandManager.Commands.ContainsKey(str);
        }

        public override bool Match(ref string str) {

            bool complete = str.Contains(' ');

            str = string.Join(' ', str.Split(' ').Skip(1).ToArray());
            return complete;
        }
    }
}
