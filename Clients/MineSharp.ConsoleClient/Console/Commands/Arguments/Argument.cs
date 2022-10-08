using PrettyPrompt.Completion;

namespace MineSharp.ConsoleClient.Console.Commands.Arguments
{
    internal abstract class Argument
    {
#pragma warning disable CS0649
        protected Action<string> ArgumentMatched;
#pragma warning restore CS0649

#pragma warning disable CS8618
        public Argument(string name, bool isOptional = false)
        {
            this.Name = name;
            this.IsOptional = isOptional;
        }
#pragma warning restore CS8618

        public string Name { get; protected set; }
        public abstract string Color { get; set; }
        public bool IsOptional { get; protected set; }
        public abstract List<CompletionItem> GetCompletionItems(string str);

        public abstract bool Match(ref string str);
        public abstract bool IsValid(string str);
    }
}
