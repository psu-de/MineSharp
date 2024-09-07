using NLog;

namespace MineSharp.Bot.Utils;

public static class LoggingHelper
{
    /// <summary>
    /// Enable debug logs of minesharp to be written to the console and a log file.
    /// Trace logs are only written to the logfile.
    /// </summary>
    /// <param name="trace">if true, also log trace messages.</param>
    public static void EnableDebugLogs(bool trace = false)
    {
        var configuration = new NLog.Config.LoggingConfiguration();

        var logfile = new NLog.Targets.FileTarget("customfile") { FileName = $"logs/{DateTime.Now:dd.MM.yyyy hh:mm:ss}.log" };
        var latestlog = new NLog.Targets.FileTarget("latestfile") { FileName = "logs/latest.log", DeleteOldFileOnStartup = true };
        var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

        var level = trace ? LogLevel.Trace : LogLevel.Debug;
        configuration.AddRule(level, LogLevel.Fatal, logfile);
        configuration.AddRule(level, LogLevel.Fatal, latestlog);
        configuration.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);

        LogManager.Configuration = configuration;
    }
}
