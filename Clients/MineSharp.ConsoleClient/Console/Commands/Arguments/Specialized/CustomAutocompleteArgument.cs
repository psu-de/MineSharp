using PrettyPrompt.Completion;

namespace MineSharp.ConsoleClient.Console.Commands.Arguments.Specialized {
    internal class CustomAutocompleteArgument : Argument {
        public override string Color { get => baseArgument.Color; set => throw new NotImplementedException(); }


        private Func<string, List<CompletionItem>> getCompletionItems;
        private Argument baseArgument;

        public CustomAutocompleteArgument(Argument baseArgument, Func<string, List<CompletionItem>> completionItems) : base(baseArgument.Name, baseArgument.IsOptional) {
            this.baseArgument = baseArgument;
            this.getCompletionItems = completionItems;
        }

        public override List<CompletionItem> GetCompletionItems(string str) {
            return this.getCompletionItems(str);
        }

        public override bool IsValid(string str) {
            return baseArgument.IsValid(str);
        }

        public override bool Match(ref string str) {
            return baseArgument.Match(ref str);
        }
    }
}
