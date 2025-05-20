namespace LoggerService
{
    public class LogTailResult
    {
        private LogTailResult(
            List<Dictionary<string, string>> logs,
            string? error,
            bool isServerError
        )
        {
            Logs = logs;
            Error = error;
            IsServerError = isServerError;
        }

        public string? Error { get; }
        public bool IsServerError { get; }
        public List<Dictionary<string, string>> Logs { get; }
        public bool Success => Error is null;

        public static LogTailResult BadRequest(string error) => new([], error, false);

        public static LogTailResult Ok(List<Dictionary<string, string>> logs) =>
            new(logs, null, false);
    }
}
