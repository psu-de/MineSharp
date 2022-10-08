using MineSharp.ConsoleClient.Console.Commands.Arguments;
using PrettyPrompt.Highlighting;
using Spectre.Console;
namespace MineSharp.ConsoleClient.Console
{
    internal abstract class Command
    {

        public void Initialize(string name, string description, string color, params Argument[] arguments)
        {

            if (this._isInitialized) return;
            this._isInitialized = true;

            description += $"\nSyntax: [{color}]{name}[/] " + string.Join(" ", arguments.Select(x => x.IsOptional ? $"[{x.Color}](<{x.Name}>)[/]" : $"[{x.Color}]<{x.Name}>[/]"));

            this.Name = name;
            this.Description = description;
            this.Color = color;
            this.Arguments = arguments.ToList();
        }

        public (Argument? arg, string remaining) GetCurrentArgument(string args)
        {
            if (!this._isInitialized) throw new InvalidOperationException("Not initialized");

            var remaining = args;
            for (var i = 0; i < this.Arguments.Count; i++)
            {
                if (!this.Arguments[i].Match(ref remaining))
                {
                    return (this.Arguments[i], remaining);
                }
            }

            return (null, "");
        }

        public List<FormatSpan> GetArgHighlighting(string args, int strOffset)
        {

            var highlights = new List<FormatSpan>();

            var strIndex = strOffset;
            for (var i = 0; i < this.Arguments.Count; i++)
            {
                var beforeArgs = args;
                var argLenBefore = args.Length;
                var complete = this.Arguments[i].Match(ref args);
                var argLen = argLenBefore - args.Length;

                if (this.Arguments[i].IsValid(beforeArgs.Substring(0, argLen).Trim()))
                {
                    highlights.Add(new FormatSpan(strIndex, argLen, CColor.GetAnsiColor(this.Arguments[i].Color)));
                } else
                {
                    highlights.Add(new FormatSpan(strIndex, argLen, AnsiColor.BrightRed));
                }
                strIndex += argLen;
            }
            return highlights;
        }

        public virtual void PrintHelp()
        {
            AnsiConsole.MarkupLine("[green]Help: " + this.Name + "[/]");
            AnsiConsole.MarkupLine($"{this.Description}");
        }

        public void Execute(string args, CancellationToken cancellation)
        {

            var argv = new List<string>();

            var remaining = args;
            for (var i = 0; i < this.Arguments.Count; i++)
            {
                remaining = remaining.TrimStart();
                var beforeArgs = remaining;
                this.Arguments[i].Match(ref remaining);
                var arg = beforeArgs.Substring(0, beforeArgs.Length - remaining.Length).Trim();

                if (string.IsNullOrEmpty(arg) && this.Arguments[i].IsOptional)
                {
                    break;
                }

                if (!this.Arguments[i].IsOptional && !this.Arguments[i].IsValid(arg))
                {
                    System.Console.WriteLine("Error: Invalid argument at position " + (i + 1));
                    return;
                }

                argv.Add(arg);
            }

            this.DoAction(argv.ToArray(), cancellation);
        }

        public abstract void DoAction(string[] argv, CancellationToken cancellation);

        #pragma warning disable CS8618

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Color { get; private set; }
        private bool _isInitialized;
        public List<Argument> Arguments;

        #pragma warning restore CS8618
    }
}
