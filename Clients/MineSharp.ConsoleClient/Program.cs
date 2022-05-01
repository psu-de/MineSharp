using MineSharp.ConsoleClient;
using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console;
using MineSharp.Data;
using PrettyPrompt;
using PrettyPrompt.Highlighting;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Net;
using System.Reflection;



//AnsiConsole.MarkupLine($"[{CColor.PlayerCommand}] Player [/]");
//AnsiConsole.MarkupLine($"[{CColor.EntityCommand}] Entity [/]");
//AnsiConsole.MarkupLine($"[{CColor.ChatCommand}] Chat [/]");
//AnsiConsole.MarkupLine($"[{CColor.MiscCommand}] Misc [/]");
//AnsiConsole.MarkupLine($"[{CColor.WorldCommand}] World [/]");
//AnsiConsole.MarkupLine($"[{CColor.PromptCommand}] Prompt [/]");

//AnsiConsole.Console.Input.ReadKey(true);

//Credentials
MineSharp.Bot.Bot.BotOptions loginOptions = new MineSharp.Bot.Bot.BotOptions();

if (true /*args.Length == 1*/) {
    loginOptions = new MineSharp.Bot.Bot.BotOptions() {
        Host = "127.0.0.1",
        Port = 25565,
        Offline = true,
        UsernameOrEmail = "lessgo",
        Version = "1.18.1"
    };
    // Parse credential file
} else {
    AnsiConsole.Write(new Rule("[yellow] MineSharp Console Client [/]").RuleStyle(Style.Parse("yellow")));
    AnsiConsole.MarkupLine("Please login:");
    string username = AnsiConsole.Ask<string>("Username [red]or[/] Email: ");
    bool isOffline = AnsiConsole.Confirm("Offline Mode: ", true);
    string? password = null;

    if (!isOffline) {
        password = AnsiConsole.Prompt(new TextPrompt<string>("Password: ").PromptStyle("red").Secret());
    }

    string host = AnsiConsole.Prompt(new TextPrompt<string>("Hostname: ")
        .ValidationErrorMessage("[red] Invalid Hostname [/]")
        .Validate(i => {
            if (IPAddress.TryParse(i, out _)) return ValidationResult.Success();
            else {
                try {
                    Dns.GetHostEntry(i);
                    return ValidationResult.Success();
                } catch (Exception) {

                    return ValidationResult.Error();
                }
            }
        }));

    ushort port = AnsiConsole.Prompt(
        new TextPrompt<ushort>("Port: ")
        .ValidationErrorMessage("[red] Invalid port number [/]"));

    string version = AnsiConsole.Ask<string>("Minecraft version: ");

    loginOptions = new MineSharp.Bot.Bot.BotOptions() {
        Host = host,
        Offline = isOffline,
        Password = password,
        Port = port,
        UsernameOrEmail = username,
        Version = version
    };
    AnsiConsole.Clear();
}

AnsiConsole.Write(new Rule("[yellow] MineSharp Console Client [/]").RuleStyle(Style.Parse("yellow")));
AnsiConsole.Write(new FigletText("MineSharp Alpha").Centered());

AnsiConsole.Status()
    .Spinner(Spinner.Known.Dots2)
    .Start($"Connecting to [purple]{loginOptions.Host}:{loginOptions.Port}[/] as [aqua]{loginOptions.UsernameOrEmail}[/]", ctx => {
        BotClient.Initialize(loginOptions);

        void botJoined() {
            ctx.Status("[green] Connected! [/]");
        }
        BotClient.Bot.Joined += botJoined;

        MinecraftData.Load();
        var successful = BotClient.Bot.Connect().GetAwaiter().GetResult();

        if (!successful) {
            AnsiConsole.MarkupLine("[red] Could not connect to server![/]");
            AnsiConsole.MarkupLine("[red] Exiting... [/]");
            Environment.Exit(0);
        } else {
            AnsiConsole.MarkupLine($"[green] Successfully connected as[/] [fuchsia]{BotClient.Bot.Session.Username}[/][green]! [/]");
        }
    });

var prompt = new Prompt(
            persistentHistoryFilepath: "./history-file",
            callbacks: new ConsoleClientCallbacks(),
            configuration: new PromptConfiguration(
                prompt: new FormattedString(">> ", new FormatSpan(0, 2, AnsiColor.Yellow)),
                completionItemDescriptionPaneBackground: AnsiColor.Rgb(30, 30, 30),
                selectedCompletionItemBackground: AnsiColor.Rgb(30, 30, 30),
                selectedTextBackground: AnsiColor.Rgb(20, 61, 102)));


while (true) {
    var response = prompt.ReadLineAsync().GetAwaiter().GetResult();
    if (response.IsSuccess) {
        if (response.Text == "exit") break;
        // optionally, use response.CancellationToken so the user can
        // cancel long-running processing of their response via ctrl-c

        string cmdL = response.Text.TrimStart();
        if (string.IsNullOrEmpty(cmdL)) continue;

        var cmdName = cmdL.Split(' ')[0];
        if (!CommandManager.TryGetCommand(cmdName, out var command)) {
            AnsiConsole.MarkupLine("[red]ERROR: Command " + cmdName + " not found![/]");
            continue;
        } else {
            var startTime = DateTime.Now;
            try {
                command.Execute(cmdL.Substring(cmdName.Length), response.CancellationToken);
            } catch (Exception ex) {
                AnsiConsole.MarkupLine("[red]An error occurred while performing command![/]");
                AnsiConsole.WriteException(ex);
            }

            if (response.CancellationToken.IsCancellationRequested) {
                AnsiConsole.MarkupLine("[red]Aborted.[/]");
                continue;
            }

            var timeTaken = DateTime.Now - startTime;
            string time = "";
            if (timeTaken.TotalMilliseconds < 1000) {
                time = $"{timeTaken.TotalMilliseconds.ToString("0.00")}ms";
            } else {
                time = $"{timeTaken.TotalSeconds.ToString("0.00")}s";
            }

            AnsiConsole.MarkupLine($"[green]Command finished in[/] [olive]{time}[/]");
        }

    }
}