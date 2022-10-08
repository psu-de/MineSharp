using PrettyPrompt.Completion;

namespace MineSharp.ConsoleClient.Console.Commands.Arguments
{
    internal class StringArgument : Argument
    {


        private string[]? Options = null;
        public StringArgument(string name, bool isOptional = false, string[]? options = null) : base(name, isOptional)
        {

            this.Options = options;
        }

        public override string Color { get => "orange1"; set => throw new NotImplementedException(); }

        public override List<CompletionItem> GetCompletionItems(string str)
        {
            if (this.Options != null)
            {
                return this.Options.Select(x => new CompletionItem(x)).ToList();
            }
            return new List<CompletionItem>();
        }

        public override bool IsValid(string str)
        {
            if (str.Contains(' '))
            {
                if (!str.StartsWith('"')) return false;
                return str.EndsWith('"');
            } else
            {
                return true;
            }

        }

        public override bool Match(ref string str)
        {
            if (str.StartsWith('"'))
            {
                var remaining = str.Substring(1);
                var idxTo = remaining.IndexOf('"');

                if (idxTo == -1)
                {
                    str = "";
                    return false;
                }
                str = remaining.Substring(idxTo + 1);
                return true;
            } else
            {
                var splits = str.Split(' ');
                if (splits.Length > 1)
                {

                    str = splits[1];
                } else str = "";
                return splits.Length > 1;
            }
        }

        public string GetValue(string val)
        {
            if (val.StartsWith('"'))
            {
                var text = val.Substring(1);
                var to = text.IndexOf('"');
                text = val.Substring(1, to);
                return text;
            } else return val;
        }
    }
}
