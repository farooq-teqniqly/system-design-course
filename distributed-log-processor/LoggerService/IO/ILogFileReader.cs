namespace LoggerService.IO;

public interface ILogFileReader
{
    Task<LogTailResult> GetLatestLogsAsync(int? requestedLimit);
}
