using PrettyPrompt.Completion;
using PrettyPrompt.Highlighting;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.ConsoleClient.Console.Commands.Arguments {
    internal abstract class Argument {

        public Argument(string name, bool isOptional = false) {
            this.Name = name;
            this.IsOptional = isOptional;
        }

        public string Name { get; protected set; }
        public abstract string Color { get; set; }
        public bool IsOptional { get; protected set; }
        public abstract List<CompletionItem> GetCompletionItems(string str);

        public abstract bool Match(ref string str);
        public abstract bool IsValid(string str);
    }
}
