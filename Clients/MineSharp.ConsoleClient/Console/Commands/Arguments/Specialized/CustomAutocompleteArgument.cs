using PrettyPrompt.Completion;
namespace MineSharp.ConsoleClient.Console.Commands.Arguments.Specialized
{
    internal class CustomAutocompleteArgument : Argument
    {
        private readonly Argument baseArgument;


        private readonly Func<string, List<CompletionItem>> getCompletionItems;

        public CustomAutocompleteArgument(Argument baseArgument, Func<string, List<CompletionItem>> completionItems) : base(baseArgument.Name, baseArgument.IsOptional)
        {
            this.baseArgument = baseArgument;
            this.getCompletionItems = completionItems;
        }
        public override string Color { get => this.baseArgument.Color; set => throw new NotImplementedException(); }

        public override List<CompletionItem> GetCompletionItems(string str) => this.getCompletionItems(str);

        public override bool IsValid(string str) => this.baseArgument.IsValid(str);

        public override bool Match(ref string str) => this.baseArgument.Match(ref str);
    }
}
