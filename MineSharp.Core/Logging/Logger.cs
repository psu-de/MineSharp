using System.Diagnostics;
using System.Reflection;

namespace MineSharp.Core.Logging {
    public class Logger {

        public static LogLevel Threshold = LogLevel.DEBUG3;

        public static TextWriter LogWriter = Console.Out;
        public static List<LogMessage> LogMessages = new List<LogMessage>();

        public delegate void LogMessageEvent(LogMessage message);
        public static event LogMessageEvent OnLogMessageReceieved;

        public struct LogMessage {
            public LogLevel Level;
            public string Caller;
            public DateTime Time;
            public string Message;

            public override string ToString() {

                string Format(string val) {
                    return val.PadLeft(2, '0');
                }

                string time = $"{Format(Time.Hour.ToString())}:{Format(Time.Minute.ToString())}:{Format(Time.Second.ToString())}";
                string logl = Level switch {
                    LogLevel.INFO => "INFO ",
                    LogLevel.DEBUG3 => "DEBUG",
                    LogLevel.DEBUG2 => "DEBUG",
                    LogLevel.DEBUG => "DEBUG",
                    LogLevel.WARN => "WARN ",
                    LogLevel.ERROR => "ERROR",
                    _ => "-----"
                };
                string name = Caller.PadRight(10, ' ').Substring(0, 10);

                return $"[{logl}][{time}][{name}] > {Message}";
            }

            public string Markup(Func<string, string>? escape = null) {
                string color = this.Level switch {
                    LogLevel.INFO => "white",
                    LogLevel.WARN => "orange1",
                    LogLevel.ERROR => "red",
                    LogLevel.DEBUG => "cyan1",
                    LogLevel.DEBUG2 => "magenta1",
                    LogLevel.DEBUG3 => "magenta2",
                };

                var str = this.ToString();
                if (escape != null)
                    str = escape(str);
                return $"[{color}]{str}[/]"; //TODO: this.ToString() escapen
            }
        }

        

        public static Logger GetLogger(string? module = null) {
            return new Logger(module ?? NamespaceOfCallingClass());
        }

        private static string NamespaceOfCallingClass() {
            Type declaringType;
            int skipFrames = 2;
            do {
                MethodBase method = new StackFrame(skipFrames, false).GetMethod();
                declaringType = method.DeclaringType;
                if (declaringType == null) {
                    return method.Name;
                }
                skipFrames++;
            }
            while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));

            string[] modules = declaringType.Namespace.Split('.');

            if (declaringType.Namespace.StartsWith("MineSharp")) {
                return modules[1];
            } else return modules.Last();
        }


        public string Name { get; private set; }

        public Logger(string name) {
            this.Name = name;
        }


        public void Debug3(string message) => this.Log(message, LogLevel.DEBUG3);
        public void Debug2(string message) => this.Log(message, LogLevel.DEBUG2);
        public void Debug(string message) => this.Log(message, LogLevel.DEBUG);
        public void Error(string message) => this.Log(message, LogLevel.ERROR);
        public void Warning(string message) => this.Log(message, LogLevel.WARN);
        public void Info(string message) => this.Log(message, LogLevel.INFO);


        public void Log(string message, LogLevel level) {
            if ((int)level > (int)Threshold) return;

            var log = new LogMessage() {
                Level = level,
                Time = DateTime.Now,
                Caller = this.Name,
                Message = message
            };


            LogWriter.WriteLine(log.ToString());
            if (level <= LogLevel.DEBUG) {
                System.Diagnostics.Debug.WriteLine(log.ToString());
            }

            if (level == LogLevel.DEBUG3) {
                LogMessages.RemoveAll(x => x.Level == LogLevel.DEBUG3 && (DateTime.Now - x.Time).TotalMinutes >= 5);
            }
            LogMessages.Add(log);
            OnLogMessageReceieved?.Invoke(log);
        }
    }
}
