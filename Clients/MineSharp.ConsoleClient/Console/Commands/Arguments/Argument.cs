using PrettyPrompt.Completion;

namespace MineSharp.ConsoleClient.Console.Commands.Arguments {
    internal abstract class Argument {

        protected Action<string> ArgumentMatched;

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
