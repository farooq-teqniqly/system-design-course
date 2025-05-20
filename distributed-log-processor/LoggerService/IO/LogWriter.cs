using LoggerService.Models;
using ILogger = Serilog.ILogger;

namespace LoggerService.IO;

public sealed class LogWriter : ILogWriter
{
    public void LogByLevel(LogEntryDto entry, ILogger logger)
    {
        switch (entry.Level.ToLowerInvariant())
        {
            case "debug":
                logger.Debug(entry.Message);
                break;
            case "information":
                logger.Information(entry.Message);
                break;
            case "warning":
                logger.Warning(entry.Message);
                break;
            case "error":
                LogError(entry, logger);
                break;
            case "fatal":
                LogFatal(entry, logger);
                break;
            default:
                logger.Information(entry.Message);
                break;
        }
    }

    private static void LogFatal(LogEntryDto entry, ILogger logger)
    {
        if (string.IsNullOrEmpty(entry.Exception))
        {
            logger.Fatal(entry.Message);
        }
        else
        {
            logger.Fatal(entry.Exception, entry.Message);
        }
    }

    private static void LogError(LogEntryDto entry, ILogger logger)
    {
        if (string.IsNullOrEmpty(entry.Exception))
        {
            logger.Error(entry.Message);
        }
        else
        {
            logger.Error(entry.Exception, entry.Message);
        }
    }
}
