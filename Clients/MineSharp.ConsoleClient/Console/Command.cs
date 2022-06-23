using MineSharp.ConsoleClient.Console.Commands.Arguments;
using PrettyPrompt.Highlighting;
using Spectre.Console;

namespace MineSharp.ConsoleClient.Console {
    internal abstract class Command {

        #pragma warning disable CS8618

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Color { get; private set; }
        private bool _isInitialized = false;
        public List<Argument> Arguments;

        #pragma warning restore CS8618

        public void Initialize(string name, string description, string color, params Argument[] arguments) {

            if (_isInitialized) return;
            _isInitialized = true;

            description += $"\nSyntax: [{color}]{name}[/] " + string.Join(" ", arguments.Select(x => x.IsOptional ? $"[{x.Color}](<{x.Name}>)[/]" : $"[{x.Color}]<{x.Name}>[/]"));

            this.Name = name;
            this.Description = description;
            this.Color = color;
            this.Arguments = arguments.ToList();
        }

        public (Argument? arg, string remaining) GetCurrentArgument(string args) {
            if (!_isInitialized) throw new InvalidOperationException("Not initialized");

            string remaining = args;
            for (int i = 0; i < Arguments.Count; i++) {
                if (!Arguments[i].Match(ref remaining)) {
                    return (Arguments[i], remaining);
                }
            }

            return (null, "");
        }

        public List<FormatSpan> GetArgHighlighting(string args, int strOffset) {

            List<FormatSpan> highlights = new List<FormatSpan>();

            int strIndex = strOffset;
            for (int i = 0; i < Arguments.Count; i++) {
                string beforeArgs = args;
                int argLenBefore = args.Length;
                bool complete = Arguments[i].Match(ref args);
                int argLen = argLenBefore - args.Length;

                if (Arguments[i].IsValid(beforeArgs.Substring(0, argLen).Trim())) {
                    highlights.Add(new FormatSpan(strIndex, argLen, CColor.GetAnsiColor(Arguments[i].Color)));
                } else {
                    highlights.Add(new FormatSpan(strIndex, argLen, AnsiColor.BrightRed));
                }
                strIndex += argLen;
            }
            return highlights;
        }

        public virtual void PrintHelp() {
            AnsiConsole.MarkupLine($"[green]Help: " + this.Name + "[/]");
            AnsiConsole.MarkupLine($"{this.Description}");
        }

        public void Execute(string args, CancellationToken cancellation) {

            List<string> argv = new List<string>();

            string remaining = args;
            for (int i = 0; i < Arguments.Count; i++) {
                remaining = remaining.TrimStart();
                string beforeArgs = remaining;
                Arguments[i].Match(ref remaining);
                string arg = beforeArgs.Substring(0, beforeArgs.Length - remaining.Length).Trim();

                if (string.IsNullOrEmpty(arg) && Arguments[i].IsOptional) {
                    break;
                }

                if (!Arguments[i].IsOptional && !Arguments[i].IsValid(arg)) {
                    System.Console.WriteLine("Error: Invalid argument at position " + (i + 1));
                    return;
                }

                argv.Add(arg);
            }

            DoAction(argv.ToArray(), cancellation);
        }

        public abstract void DoAction(string[] argv, CancellationToken cancellation);
    }
}
