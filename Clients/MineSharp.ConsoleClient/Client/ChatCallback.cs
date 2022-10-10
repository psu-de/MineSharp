using MineSharp.Bot;
using MineSharp.Core.Logging;
using MineSharp.Core.Types;
using Spectre.Console;

namespace MineSharp.ConsoleClient.Client
{
    public class ChatCallback
    {
        private static readonly Logger Logger = Logger.GetLogger();

        private Action<string, CancellationToken> ExecutionFunc;
        
        public ChatCallback(MinecraftBot bot, Action<string, CancellationToken> executionFunc)
        {
            this.ExecutionFunc = executionFunc;
            bot.ChatMessageReceived += this.OnChatReceived;
        }

        private void OnChatReceived(MinecraftBot bot, string message, MinecraftPlayer messageSender)
        {
            Logger.Info($"Chat: <{messageSender.Username}> {message}");
            
            if (messageSender.Username != bot.Player!.Username)
            {
                AnsiConsole.WriteLine(message);
                this.ExecutionFunc(message, CancellationToken.None);
                // TODO: Execute commands
            }
        }
    }
}
