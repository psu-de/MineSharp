using MineSharp.Bot;
using MineSharp.Core.Types;
using Spectre.Console;

namespace MineSharp.ConsoleClient.Client;

public class ChatCallback
{

    public ChatCallback(MinecraftBot bot)
    {
        bot.ChatReceived += OnChatReceived;
    }
    
    private void OnChatReceived(MinecraftBot bot, Chat chat, MinecraftPlayer messageSender)
    {
        if (messageSender.Username != bot.Player!.Username)
        {
            // TODO: Parse the chat object
        }
    }
    
}