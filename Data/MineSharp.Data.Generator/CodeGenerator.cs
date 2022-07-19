using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Data.Generator {
    internal class CodeGenerator {

        public const string Indent = "\t";
        public const string NewLine = "\r\n";

        private int IndentCount = 0;
        public string CurrentIndent => string.Join("", Enumerable.Repeat(Indent, IndentCount));

        private StringBuilder Builder;

        public CodeGenerator() {
            Builder = new StringBuilder();
        }

        public void ClearIndent() => IndentCount = 0;
        public void PushIndent() => IndentCount++;
        public void PopIndent() {
            IndentCount--;
            if (IndentCount < 0) throw new Exception();
        }

        public void WriteLine() {
            Builder.AppendLine();
        }
        public void WriteLine(string line) {
            Builder.AppendLine(CurrentIndent + line);
        }

        public void WriteBlock(string str) {
            string[] lines = str.Split("\r\n");
            foreach (var line in lines) {
                WriteLine(line);
            }
        }

        public override string ToString() {
            return Builder.ToString();
        }

        private int ScopeStack = 0;
        public void Begin(string line) {
            WriteLine(line + " {");
            ScopeStack++;
            PushIndent();
        }

        public void Finish(bool semicolon = false) {
            if (ScopeStack <= 0) throw new Exception();
            PopIndent();
            WriteLine("}" + (semicolon ? ";" : ""));
            ScopeStack--;
        }

        public void CommentBlock(string comment) {
            WriteLine(new string(Enumerable.Repeat('/', comment.Length + 10).ToArray()));
            WriteLine($"//   {comment}   //");
            WriteLine(new string(Enumerable.Repeat('/', comment.Length + 10).ToArray()));
        }
    }
}
