using MineSharp.Bot;
using MineSharp.ConsoleClient;
using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console;
using MineSharp.Core.Logging;
using PrettyPrompt;
using PrettyPrompt.Highlighting;
using Spectre.Console;
using System.Net;
using MineSharp.MojangAuth;

const string DEBUG_LOG_FILE = "log_debug.txt";
const string LOG_FILE = "log.txt";
const bool APPEND_LOG = false;

var logfileWriter = new StreamWriter(LOG_FILE, APPEND_LOG) {
    AutoFlush = true
};
var debugFileWriter = new StreamWriter(DEBUG_LOG_FILE, APPEND_LOG) {
    AutoFlush = true
};
Logger.AddScope(LogLevel.DEBUG, s => logfileWriter.WriteLine(s));
Logger.AddScope(LogLevel.DEBUG3, s => debugFileWriter.WriteLine(s));

//Credentials
var loginOptions = new MinecraftBot.BotOptions();
bool offline = AnsiConsole.Confirm("Use offline mode", true);
loginOptions.Offline = offline;
loginOptions.Host = "127.0.0.1";
loginOptions.Port = 25565;

AnsiConsole.Write(new Rule("[yellow] MineSharp Console Client [/]").RuleStyle(Style.Parse("yellow")));
AnsiConsole.Write(new FigletText("MineSharp Alpha").Centered());

var thresholdLogLevel = LogLevel.INFO;

void LogMessage(Logger.LogMessage log)
{
    if (log.Level <= thresholdLogLevel)
    {
        AnsiConsole.MarkupLine(log.Markup(Markup.Escape));
    }
}

AnsiConsole.Status()
    .Spinner(Spinner.Known.Dots2)
    .Start($"Connecting to [purple]{loginOptions.Host}:{loginOptions.Port}[/] as [aqua]{loginOptions.UsernameOrEmail}[/]", ctx =>
    {
        BotClient.Initialize(loginOptions, ExecuteCommand);
        Logger.OnLogMessageReceieved += LogMessage;

        var successful = BotClient.Bot!.Connect().GetAwaiter().GetResult();
        thresholdLogLevel = LogLevel.ERROR;

        if (!successful)
        {
            AnsiConsole.MarkupLine("[red] Could not connect to server![/]");
            AnsiConsole.MarkupLine("[red] Exiting... [/]");
            Environment.Exit(0);
        } else
        {
            AnsiConsole.MarkupLine($"[green] Successfully connected as[/] [fuchsia]{BotClient.Bot.Session.Username}[/][green]! [/]");
        }
    });

var prompt = new Prompt(
    "./history-file",
    new ConsoleClientCallbacks(),
    configuration: new PromptConfiguration(
        prompt: new FormattedString(">> ", new FormatSpan(0, 2, AnsiColor.Yellow)),
        completionItemDescriptionPaneBackground: AnsiColor.Rgb(30, 30, 30),
        selectedCompletionItemBackground: AnsiColor.Rgb(30, 30, 30),
        selectedTextBackground: AnsiColor.Rgb(20, 61, 102)));

while (true)
{
    var response = prompt.ReadLineAsync().GetAwaiter().GetResult();
    if (response.IsSuccess)
    {
        if (response.Text == "exit") break;
        // optionally, use response.CancellationToken so the user can
        // cancel long-running processing of their response via ctrl-c

        ExecuteCommand(response.Text, response.CancellationToken);
    }
}

// TODO: Maybe check if a command is already running
void ExecuteCommand(string cmdL, CancellationToken cancellationToken)
{
    if (string.IsNullOrEmpty(cmdL)) return;

    var cmdName = cmdL.Split(' ')[0];
    if (!CommandManager.TryGetCommand(cmdName, out var command))
    {
        AnsiConsole.MarkupLine("[red]ERROR: Command " + cmdName + " not found![/]");
        return;
    }
    var startTime = DateTime.Now;
    try
    {
        command.Execute(cmdL.Substring(cmdName.Length), cancellationToken);
    } catch (Exception ex)
    {
        AnsiConsole.MarkupLine("[red]An error occurred while performing command![/]");
        AnsiConsole.WriteException(ex);
    }

    if (cancellationToken.IsCancellationRequested)
    {
        AnsiConsole.MarkupLine("[red]Aborted.[/]");
        return;
    }

    var timeTaken = DateTime.Now - startTime;
    var time = "";
    if (timeTaken.TotalMilliseconds < 1000)
    {
        time = $"{timeTaken.TotalMilliseconds.ToString("0.00")}ms";
    } else
    {
        time = $"{timeTaken.TotalSeconds.ToString("0.00")}s";
    }

    AnsiConsole.MarkupLine($"[green]Command finished in[/] [olive]{time}[/]");
}
