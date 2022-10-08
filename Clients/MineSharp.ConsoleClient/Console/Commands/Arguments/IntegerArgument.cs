using PrettyPrompt.Completion;

namespace MineSharp.ConsoleClient.Console.Commands.Arguments
{
    internal class IntegerArgument : Argument
    {

        public IntegerArgument(string name, bool isOptional = false) : base(name, isOptional) {}

        public override string Color { get => "lime"; set => throw new NotImplementedException(); }

        public override List<CompletionItem> GetCompletionItems(string str) => new List<CompletionItem>();

        public override bool IsValid(string str) => int.TryParse(str, out var value);

        public override bool Match(ref string str)
        {
            var complete = str.Contains(' ');

            str = string.Join(' ', str.Split(' ').Skip(1).ToArray());
            return complete;
        }

        public int? GetValue(string str)
        {
            if (!int.TryParse(str, out var value))
            {
                return null;
            } else return value;
        }
    }
}
