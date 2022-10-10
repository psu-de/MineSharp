/*
 *  How to use:
 *    - When starting the bot will connect to 127.0.0.1:25565 
 *    - Enter a message in the minecraft chat with a different player,
 *      and the bot will echo the message
 */

using MineSharp.Bot;
using MineSharp.Core.Types;

var bot = new MinecraftBot(new MinecraftBot.BotOptions() {
    Host = "127.0.0.1",
    Offline = true,
    Port = 25565,
    Version = "1.18.1",
    UsernameOrEmail = "MineSharpBot"
});
Console.CancelKeyPress += OnCtrlC;

_ = Task.Run(async () =>                                                        // Must run in another Task to prevent blocking
{
    Console.WriteLine("Connecting...");
    bool successful = await bot.Connect();                                      // Connect to the server

    if (!successful)
    {
        Console.WriteLine("Could not connect to server");
        Environment.Exit(0);
    }
    Console.WriteLine("Connected!");

    await bot.WaitForBot();                                                     // Wait until the bot entity is initialized
    bot.Died += (sender, chat) => bot.Respawn();                     // Auto Respawn the bot when he dies
    
    bot.ChatMessageReceived += OnMessageReceived;                               // Event fired when a chat message is received

    void OnMessageReceived(MinecraftBot sender, string message, MinecraftPlayer messageSender)
    {
        if (messageSender.Username != sender.Player!.Username)
        {
            sender.Chat(message);                                               // Echo the message back
        }
    }
});

void OnCtrlC(object? sender, ConsoleCancelEventArgs e)
{
    Console.WriteLine("Exiting...");
    bot.Client.ForceDisconnect("");
    Task.Delay(1000).Wait();
    Environment.Exit(0);
}

Console.ReadKey();
