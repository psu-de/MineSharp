using MineSharp.Bot;
using MineSharp.Core.Logging;
using MineSharp.Core.Types;

namespace MineSharp.ConsoleClient.Client
{
    public class ChatCallback
    {
        private static readonly Logger Logger = Logger.GetLogger();
        public ChatCallback(MinecraftBot bot)
        {
            bot.ChatReceived += this.OnChatReceived;
        }

        private void OnChatReceived(MinecraftBot bot, Chat chat, MinecraftPlayer messageSender)
        {
            Logger.Info($"Chat: " + chat.Message);
            
            if (messageSender.Username != bot.Player!.Username)
            {
                // TODO: Execute commands
            }
        }
    }
}
