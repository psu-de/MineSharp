using PrettyPrompt.Completion;

namespace MineSharp.ConsoleClient.Console.Commands.Arguments {
    internal class BoolArgument : Argument {
        public override string Color { get => "dodgerblue3"; set => throw new NotImplementedException(); }

        public BoolArgument(string name, bool isOptional = false) : base(name, isOptional) { }

        public override List<CompletionItem> GetCompletionItems(string str) {
            return new List<CompletionItem>() {
                new CompletionItem(
                    replacementText: "true",
                    displayText: CColor.FromMarkup($"[{Color}]true[/]")),
                new CompletionItem(
                    replacementText: "false",
                    displayText: CColor.FromMarkup($"[{Color}]false[/]"))
            };
        }

        public override bool IsValid(string str) {
            return bool.TryParse(str, out var _);
        }

        public override bool Match(ref string str) {
            bool complete = str.Contains(' ');

            str = string.Join(' ', str.Split(' ').Skip(1).ToArray());
            return complete;
        }

        public bool GetValue(string str) {
            return bool.Parse(str);
        }
    }
}
