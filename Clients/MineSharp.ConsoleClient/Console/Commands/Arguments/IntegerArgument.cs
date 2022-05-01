using PrettyPrompt.Completion;
using PrettyPrompt.Highlighting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.ConsoleClient.Console.Commands.Arguments {
    internal class IntegerArgument : Argument {

        public IntegerArgument(string name, bool isOptional = false) : base(name, isOptional) { }  

        public override string Color { get => "lime"; set => throw new NotImplementedException(); }

        public override List<CompletionItem> GetCompletionItems(string str) {
            return new List<CompletionItem>();
        }

        public override bool IsValid(string str) {
            return int.TryParse(str, out int value);
        }

        public override bool Match(ref string str) {
            bool complete = str.Contains(' ');

            str = string.Join(' ', str.Split(' ').Skip(1).ToArray());
            return complete;
        }

        public int? GetValue(string str) {
            if (!int.TryParse(str, out int value)) {
                return null;
            } else return value;
        }
    }
}
