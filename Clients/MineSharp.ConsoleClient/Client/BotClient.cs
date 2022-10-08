using MineSharp.Bot;
using MineSharp.Core.Logging;

namespace MineSharp.ConsoleClient.Client
{
    public static class BotClient
    {

        public static MemoryStream BotLog = new MemoryStream();
        private static StreamWriter? BotLogWriter;
        private static ChatCallback ChatCallback;

        public static MinecraftBot? Bot;

        public static void Initialize(MinecraftBot.BotOptions options)
        {
            BotLogWriter = new StreamWriter(BotLog) {
                AutoFlush = true
            };

            Logger.AddScope(LogLevel.DEBUG3, s => BotLogWriter.WriteLine(s));
            Bot = new MinecraftBot(options);

            _ = Bot.WaitForBot().ContinueWith(v =>
            {
                ChatCallback = new ChatCallback(Bot);
            });

        }
    }
}
