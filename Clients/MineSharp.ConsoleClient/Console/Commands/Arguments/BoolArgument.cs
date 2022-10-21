using PrettyPrompt.Completion;

namespace MineSharp.ConsoleClient.Console.Commands.Arguments
{
    internal class BoolArgument : Argument
    {

        public BoolArgument(string name, bool isOptional = false) : base(name, isOptional) {}
        public override string Color { get => "dodgerblue3"; set => throw new NotImplementedException(); }

        public override List<CompletionItem> GetCompletionItems(string str) => new List<CompletionItem> {
            new CompletionItem(
                "true",
                CColor.FromMarkup($"[{this.Color}]true[/]")),
            new CompletionItem(
                "false",
                CColor.FromMarkup($"[{this.Color}]false[/]"))
        };

        public override bool IsValid(string str) => bool.TryParse(str, out var _);

        public override bool Match(ref string str)
        {
            var complete = str.Contains(' ');

            str = string.Join(' ', str.Split(' ').Skip(1).ToArray());
            return complete;
        }

        public bool GetValue(string str) => bool.Parse(str);
    }
}
