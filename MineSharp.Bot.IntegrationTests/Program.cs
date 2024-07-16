using MineSharp.Bot;
using MineSharp.Bot.IntegrationTests.Tests;
using MineSharp.Bot.Plugins;
using MineSharp.ChatComponent;
using MineSharp.Data;
using Newtonsoft.Json.Linq;

var bot = await new BotBuilder()
               .Host("localhost")
               .OfflineSession("MineSharpBot")
               .AutoConnect()
               .CreateAsync();

var chat = bot.GetPlugin<ChatPlugin>();
chat.OnChatMessageReceived += (sender, player, message, position, name) =>
{
    Console.WriteLine($"Player={player}");
    Console.WriteLine($"Message={message.GetMessage(bot.Data)}");
    Console.WriteLine($"Position={position}");
    Console.WriteLine($"Name={name}");
};

while (true)
{
    var msg = Console.ReadLine();

    if (string.IsNullOrEmpty(msg))
        continue;

    if (msg == "exit")
        break;
    
    await chat.SendChat(msg);
}


//
// await PlayerTests.RunAll();
// await WindowTests.RunAll();
// await WorldTests.RunAll();
// await CraftingTests.RunAll();
// await PhysicsTests.RunAll();
