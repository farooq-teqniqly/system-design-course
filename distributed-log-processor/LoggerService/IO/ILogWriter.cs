using LoggerService.Models;
using ILogger = Serilog.ILogger;

namespace LoggerService.IO;

public interface ILogWriter
{
    void LogByLevel(LogEntryDto entry, ILogger logger);
}