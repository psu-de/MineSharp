using System.Diagnostics;
using System.Reflection;

namespace MineSharp.Core.Logging {
    public class Logger {

        public static LogLevel Threshold = LogLevel.DEBUG3;

        public static TextWriter LogWriter = Console.Out;

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

        public void Error(string message) => this.Log(message, LogLevel.Error);

        public void Warning(string message) => this.Log(message, LogLevel.WARN);

        public void Info(string message) => this.Log(message, LogLevel.INFO);


        public void Log(string message, LogLevel level) {
            if ((int)level > (int)Threshold) return;
            string time = this.GetTimeFormatted();
            string logl = this.GetLogLevelString(level);
            string name = this.Name.PadRight(10, ' ').Substring(0, 10);

            string logMessage = $"[{logl}][{time}][{name}] > {message}";

            LogWriter.WriteLine(logMessage);
            if (level <= LogLevel.DEBUG) {
                System.Diagnostics.Debug.WriteLine(logMessage);
            }
        }

        private string GetLogLevelString(LogLevel level) {
            switch (level) {
                case LogLevel.INFO:  return "INFO ";
                case LogLevel.DEBUG3:
                case LogLevel.DEBUG2:
                case LogLevel.DEBUG: return "DEBUG";
                case LogLevel.WARN:  return "WARN ";
                case LogLevel.Error: return "ERROR";
                default: return "-----";
            }
        }

         private string GetTimeFormatted() {

            string Format(string val) {
                return val.PadLeft(2, '0');
            }

            DateTime dt = DateTime.Now;
            return $"{Format(dt.Hour.ToString())}:{Format(dt.Minute.ToString())}:{Format(dt.Second.ToString())}";

         }
    }
}
