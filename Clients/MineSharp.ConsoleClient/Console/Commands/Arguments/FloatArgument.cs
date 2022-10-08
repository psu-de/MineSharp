using PrettyPrompt.Completion;

namespace MineSharp.ConsoleClient.Console.Commands.Arguments
{
    internal class FloatArgument : Argument
    {

        public FloatArgument(string name, bool isOptional = false) : base(name, isOptional) {}

        public override string Color { get => "teal"; set => throw new NotImplementedException(); }

        public override List<CompletionItem> GetCompletionItems(string str) => new List<CompletionItem>();

        public override bool IsValid(string str) => float.TryParse(str, out var value);

        public override bool Match(ref string str)
        {
            var complete = str.Contains(' ');

            str = string.Join(' ', str.Split(' ').Skip(1).ToArray());
            return complete;
        }

        public float? GetValue(string str)
        {
            if (!float.TryParse(str, out var value))
            {
                return null;
            } else return value;
        }

    }
}
