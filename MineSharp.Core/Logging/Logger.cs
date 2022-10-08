using System.Diagnostics;
namespace MineSharp.Core.Logging
{
    public class Logger
    {

        public delegate void LogMessageEvent(LogMessage message);

        public static LogLevel Threshold = LogLevel.DEBUG3;

        internal static List<LogScope> Scopes = new List<LogScope>();
        public static List<LogMessage> LogMessages = new List<LogMessage>();

        public Logger(string name)
        {
            this.Name = name;
        }


        public string Name {
            get;
        }
        public static event LogMessageEvent? OnLogMessageReceieved;

        public static void AddScope(LogLevel threshold, Action<string> writeLine)
        {
            Scopes.Add(new LogScope(threshold, writeLine));
        }


        public static Logger GetLogger(string? module = null) => new Logger(module ?? NamespaceOfCallingClass());

        private static string NamespaceOfCallingClass()
        {
            Type declaringType;
            var skipFrames = 2;
            do
            {
                var method = new StackFrame(skipFrames, false).GetMethod()!;
                declaringType = method.DeclaringType!;
                if (declaringType == null)
                {
                    return method.Name;
                }
                skipFrames++;
            }
            while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));

            var modules = declaringType.Namespace!.Split('.');

            if (declaringType.Namespace.StartsWith("MineSharp"))
            {
                return modules[1];
            }
            return modules.Last();
        }


        public void Debug3(string message) => this.Log(message, LogLevel.DEBUG3);
        public void Debug2(string message) => this.Log(message, LogLevel.DEBUG2);
        public void Debug(string message) => this.Log(message, LogLevel.DEBUG);
        public void Error(string message) => this.Log(message, LogLevel.ERROR);
        public void Warning(string message) => this.Log(message, LogLevel.WARN);
        public void Info(string message) => this.Log(message, LogLevel.INFO);


        public void Log(string message, LogLevel level)
        {
            if ((int)level > (int)Threshold) return;

            var log = new LogMessage {
                Level = level,
                Time = DateTime.Now,
                Caller = this.Name,
                Message = message
            };

            var msg = log.ToString();
            foreach (var scope in Scopes)
            {
                if (level <= scope.Threshold)
                {
                    scope.WriteLine(msg);
                }
            }

            if (level == LogLevel.DEBUG3)
            {
                LogMessages.RemoveAll(x => x.Level == LogLevel.DEBUG3 && (DateTime.Now - x.Time).TotalMinutes >= 5);
            }
            LogMessages.Add(log);
            OnLogMessageReceieved?.Invoke(log);
        }

        public struct LogMessage
        {
            public LogLevel Level;
            public string Caller;
            public DateTime Time;
            public string Message;

            private string Format(string val) => val.PadLeft(2, '0');

            public override string ToString()
            {
                var time = $"{this.Format(this.Time.Hour.ToString())}:{this.Format(this.Time.Minute.ToString())}:{this.Format(this.Time.Second.ToString())}";
                var logl = this.Level switch {
                    LogLevel.INFO => "INFO ",
                    LogLevel.DEBUG3 => "DEBUG",
                    LogLevel.DEBUG2 => "DEBUG",
                    LogLevel.DEBUG => "DEBUG",
                    LogLevel.WARN => "WARN ",
                    LogLevel.ERROR => "ERROR",
                    _ => "-----"
                };
                var name = this.Caller.PadRight(10, ' ').Substring(0, 10);

                return $"[{logl}][{time}][{name}] > {this.Message}";
            }

            public string Markup(Func<string, string>? escape = null)
            {
                var color = this.Level switch {
                    LogLevel.INFO => "white",
                    LogLevel.WARN => "orange1",
                    LogLevel.ERROR => "red",
                    LogLevel.DEBUG => "cyan1",
                    LogLevel.DEBUG2 => "magenta1",
                    LogLevel.DEBUG3 => "magenta2",
                    _ => throw new Exception()
                };

                var str = this.ToString();
                if (escape != null)
                    str = escape(str);
                return $"[{color}]{str}[/]";
            }
        }
    }
}
