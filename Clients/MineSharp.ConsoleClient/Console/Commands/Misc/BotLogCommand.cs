using MineSharp.ConsoleClient.Console.Commands.Arguments;
using MineSharp.Core.Logging;
using Spectre.Console;

namespace MineSharp.ConsoleClient.Console.Commands.Misc
{
    internal class BotLogCommand : Command
    {

        private EnumArgument<LogLevel> LogLevelArg = new EnumArgument<LogLevel>("logLevel", true);
        private IntegerArgument CountArg = new IntegerArgument("count", true);

        public BotLogCommand()
        {
            var desc = $"Shows [{this.CountArg.Color}]count[/] messages of the internal log of the bot. Optionally filtered by [{this.LogLevelArg.Color}]LogLevel[/]";

            this.Initialize("botLog", desc, CColor.MiscCommand, this.LogLevelArg, this.CountArg);
        }

        public override void PrintHelp()
        {
            base.PrintHelp();

            AnsiConsole.WriteLine($"Use count=0 for infinite messages");
            AnsiConsole.WriteLine($"Use count<0 for the last [{this.CountArg.Color}]count[/] messages");
        }

        public override void DoAction(string[] argv, CancellationToken cancellation)
        {

            var count = 0;
            LogLevel? level = null;
            if (argv.Length > 0)
            {
                level = this.LogLevelArg.GetValue(argv[0]);
            }

            if (argv.Length > 1)
            {
                var c = this.CountArg.GetValue(argv[1]);
                if (c.HasValue) count = c.Value;
            }

            var messages = Logger.LogMessages;
            if (level.HasValue)
            {
                AnsiConsole.WriteLine(level.Value.ToString());
                messages = messages.Where(x => (int)x.Level <= (int)level.Value).ToList();
            }

            if (count != 0)
            {
                if (count < 0)
                {
                    messages = messages.Skip(messages.Count + count).ToList();
                } else
                {
                    messages = messages.Take(count).ToList();
                }
            }

            foreach (var message in messages)
            {
                AnsiConsole.MarkupLine(message.Markup(Markup.Escape));
            }
        }
    }
}
