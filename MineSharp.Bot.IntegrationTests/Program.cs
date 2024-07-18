using MineSharp.Bot;
using MineSharp.Bot.Plugins;

// var bytes  = File.ReadAllBytes("player_chat_packet.bin");
// var buffer = new PacketBuffer(bytes, ProtocolVersion.V_1_20_3);
//
// Console.WriteLine(buffer.ReadUuid());
// Console.WriteLine(buffer.ReadVarInt());
// Console.WriteLine(buffer.ReadBool() ? buffer.ReadBytes(new byte[256]) : null);
// Console.WriteLine(buffer.ReadString());
// Console.WriteLine(buffer.ReadLong());
// Console.WriteLine(buffer.ReadLong());
//

var bot = await new BotBuilder()
               .Host("localhost")
               .OnlineSession("MineSharpBot")
               .AutoConnect()
               .CreateAsync();

var chat = bot.GetPlugin<ChatPlugin>();
chat.OnChatMessageReceived += (sender, player, message, position) =>
{
    Console.WriteLine($"Player={player}");
    Console.WriteLine($"Message={message.GetMessage(bot.Data)}");
    Console.WriteLine($"Position={Enum.GetName(position)}");
};

while (true)
{
    var msg = Console.ReadLine();

    if (string.IsNullOrEmpty(msg))
    {
        continue;
    }

    if (msg == "exit")
    {
        break;
    }

    await chat.SendChat(msg);
}


//
// await PlayerTests.RunAll();
// await WindowTests.RunAll();
// await WorldTests.RunAll();
// await CraftingTests.RunAll();
// await PhysicsTests.RunAll();
