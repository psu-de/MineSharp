using Humanizer;
using MineSharp.Bot.Plugins;
using Spectre.Console;

namespace MineSharp.Bot.IntegrationTests;

public static class IntegrationTest
{
    private const string HOST = "localhost";
    private const ushort PORT = 25565;

    public delegate Task TestFunction(MinecraftBot bot, TaskCompletionSource<bool> source);
    
    public static async Task RunTest(string testName, TestFunction callback, int timeout = 10 * 1000, int? commandDelay = null)
    {
        var bot = await MinecraftBot.CreateBot("MineSharpBot", HOST, PORT, offline: true);
        var chat = bot.GetPlugin<ChatPlugin>();

        if (!await bot.Connect())
        {
            AnsiConsole.MarkupLine("[red]ERROR: Could not connect to server. Is the sever running?[/]");
            Environment.Exit(1);
        }

        await Task.Delay(1000);

        // 'Reset' player state
        await chat.SendChat("/clear");
        await chat.SendChat("/kill");
        await Task.Delay(1000);
        await bot.GetPlugin<PlayerPlugin>().Respawn();

        await Task.Delay(3 * 1000);

        var tsc = new TaskCompletionSource<bool>();
        var test = callback(bot, tsc);

        if (commandDelay.HasValue)
            await Task.Delay(commandDelay.Value);
        
        await chat.SendChat($"/trigger {testName}");
        await Task.WhenAny(tsc.Task, Task.Delay(timeout));

        if (test.Exception != null)
        {
            throw test.Exception;
        }
        
        string team;
        if (tsc.Task.IsCompleted)
        {
            AnsiConsole.MarkupLine(
                tsc.Task.Result 
                    ? $"[lime]Test success: {testName}[/]"
                    : $"[red1]Test failed: {testName}[/]" );
            team = tsc.Task.Result ? "Successful" : "Failed";
        }
        else
        {
            AnsiConsole.MarkupLine($"[orange3]Test timed out: {testName}[/]");
            team = "TimedOut";
        }

        await chat.SendChat($"/scoreboard players set {testName.Pascalize()} Tests 0");
        await chat.SendChat($"/team join {team} {testName.Pascalize()}");

        await bot.Disconnect();
    }
}
