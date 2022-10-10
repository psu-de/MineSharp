using MineSharp.Bot;
using MineSharp.Core.Logging;

namespace MineSharp.ConsoleClient.Client
{
    public static class BotClient
    {

        public static MemoryStream BotLog = new MemoryStream();
        private static StreamWriter? BotLogWriter;
        private static ChatCallback ChatCallback;
        private static Action<string, CancellationToken> ExecutionFunc;
        
        public static MinecraftBot? Bot;

        public static void Initialize(MinecraftBot.BotOptions options, Action<string, CancellationToken> executionFunc)
        {
            ExecutionFunc = executionFunc;
            BotLogWriter = new StreamWriter(BotLog) {
                AutoFlush = true
            };

            Logger.AddScope(LogLevel.DEBUG3, s => BotLogWriter.WriteLine(s));
            Bot = new MinecraftBot(options);

            _ = Bot.WaitForBot().ContinueWith(v =>
            {
                ChatCallback = new ChatCallback(Bot, ExecutionFunc);
            });
            
        }
    }
}
