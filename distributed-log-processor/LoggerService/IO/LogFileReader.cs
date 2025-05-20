using System.Text.Json;
using LoggerService.Configuration;
using Microsoft.Extensions.Options;

namespace LoggerService.IO
{
    public class LogFileReader : ILogFileReader
    {
        private readonly LoggerConfiguration _loggerConfiguration;
        private readonly LogTailOptions _options;

        public LogFileReader(
            IConfiguration config,
            IOptions<LogTailOptions> options,
            LoggerConfiguration loggerConfiguration
        )
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

            _loggerConfiguration =
                loggerConfiguration ?? throw new ArgumentNullException(nameof(loggerConfiguration));
        }

        public async Task<LogTailResult> GetLatestLogsAsync(int? requestedLimit)
        {
            int limit = _options.ClampLimit(requestedLimit);

            var logsDirectory = _loggerConfiguration.LogsDirectory;

            if (!Directory.Exists(logsDirectory))
            {
                return LogTailResult.BadRequest($"Logs directory not found: {logsDirectory}");
            }

            var logFilePattern = _loggerConfiguration.LogFilePattern;

            var files = Directory.GetFiles(logsDirectory, logFilePattern);

            if (files.Length == 0)
            {
                return LogTailResult.Ok([]);
            }

            var logFile = files.OrderByDescending(f => f).FirstOrDefault();

            if (logFile == null)
            {
                return LogTailResult.Ok([]);
            }

            var logs = await ReadLastNLinesAsync(logFile, limit);

            return LogTailResult.Ok(logs);
        }

        private static async Task<List<Dictionary<string, string>>> ReadLastNLinesAsync(
            string logFile,
            int limit
        )
        {
            var logs = new LinkedList<Dictionary<string, string>>();

            await using (
                var fs = new FileStream(
                    logFile,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite
                )
            )
            {
                using (var sr = new StreamReader(fs))
                {
                    return await ReadLogLinesWithLimitAsync(limit, sr, logs);
                }
            }
        }

        private static async Task<List<Dictionary<string, string>>> ReadLogLinesWithLimitAsync(
            int limit,
            StreamReader sr,
            LinkedList<Dictionary<string, string>> logs
        )
        {
            while (!sr.EndOfStream)
            {
                var line = await sr.ReadLineAsync();

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var flatDict = TryParseLogLine(line);

                if (flatDict == null)
                {
                    continue;
                }

                logs.AddLast(flatDict);

                if (logs.Count > limit)
                {
                    logs.RemoveFirst();
                }
            }

            return [.. logs];
        }

        private static Dictionary<string, string>? TryParseLogLine(string line)
        {
            try
            {
                var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(line);
                return dict?.ToDictionary(kv => kv.Key, kv => kv.Value.ToString());
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, "Failed to deserialize log line: {Line}", line);
                return null;
            }
        }
    }
}
