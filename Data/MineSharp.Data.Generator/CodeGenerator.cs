using System.Text;

namespace MineSharp.Data.Generator
{
    internal class CodeGenerator
    {

        public const string Indent = "\t";
        public const string NewLine = "\r\n";

        private int IndentCount = 0;
        public string CurrentIndent => string.Join("", Enumerable.Repeat(Indent, this.IndentCount));

        private StringBuilder Builder;

        public CodeGenerator()
        {
            this.Builder = new StringBuilder();
        }

        public void ClearIndent() => this.IndentCount = 0;
        public void PushIndent() => this.IndentCount++;
        public void PopIndent()
        {
            this.IndentCount--;
            if (this.IndentCount < 0) throw new Exception();
        }

        public void WriteLine()
        {
            this.Builder.AppendLine();
        }
        public void WriteLine(string line)
        {
            this.Builder.AppendLine(this.CurrentIndent + line);
        }

        public void WriteBlock(string str)
        {
            var lines = str.Split("\r\n");
            foreach (var line in lines)
            {
                this.WriteLine(line);
            }
        }

        public override string ToString() => this.Builder.ToString();

        private int ScopeStack = 0;
        public void Begin(string line)
        {
            this.WriteLine(line + " {");
            this.ScopeStack++;
            this.PushIndent();
        }

        public void Finish(bool semicolon = false)
        {
            if (this.ScopeStack <= 0) throw new Exception();
            this.PopIndent();
            this.WriteLine("}" + (semicolon ? ";" : ""));
            this.ScopeStack--;
        }

        public void CommentBlock(string comment)
        {
            this.WriteLine(new string(Enumerable.Repeat('/', comment.Length + 10).ToArray()));
            this.WriteLine($"//   {comment}   //");
            this.WriteLine(new string(Enumerable.Repeat('/', comment.Length + 10).ToArray()));
        }
    }
}
