using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MineSharp.Bot;


namespace MineSharp.ConsoleClient.Client {
    public static class BotClient {

        public static MemoryStream BotLog = new MemoryStream();
        private static StreamWriter BotLogWriter;

        public static void Initialize (Bot.Bot.BotOptions options) {
            BotLogWriter = new StreamWriter(BotLog);
            BotLogWriter.AutoFlush = true;

            MineSharp.Core.Logging.Logger.LogWriter = BotLogWriter;
            Bot = new Bot.Bot(options);
        }

        public static Bot.Bot Bot;

    }
}
