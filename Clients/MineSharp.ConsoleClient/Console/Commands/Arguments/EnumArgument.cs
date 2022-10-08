using PrettyPrompt.Completion;
namespace MineSharp.ConsoleClient.Console.Commands.Arguments
{
    internal class EnumArgument<T> : Argument where T : Enum
    {

        public EnumArgument(string name, bool isOptional = false) : base(name, isOptional) {}

        public override string Color { get => "cornflowerblue"; set => throw new NotImplementedException(); }

        public override List<CompletionItem> GetCompletionItems(string str)
        {
            return Enum.GetNames(typeof(T)).Select(x =>
            {
                return new CompletionItem(
                    x,
                    CColor.FromMarkup($"[{this.Color}]{x}[/]")
                    );
            }).ToList();
        }

        public override bool IsValid(string str) => Enum.TryParse(typeof(T), str, out var _);

        public override bool Match(ref string str)
        {
            var complete = str.Contains(' ');

            str = string.Join(' ', str.Split(' ').Skip(1).ToArray());
            return complete;
        }

        public T? GetValue(string str)
        {
            if (!Enum.TryParse(typeof(T), str, out var bt))
                return default(T);
            return (T?)bt;
        }
    }
}
