namespace LoggerService;

public interface ILogFileReader
{
    Task<LogTailResult> GetLatestLogsAsync(int? requestedLimit);
}
