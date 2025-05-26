using Microsoft.Extensions.Options;

namespace Sdc.LoggingService
{
    public sealed class Logger
    {
        private readonly LoggingOptions _loggingOptions;

        public Logger(IOptions<LoggingOptions> loggingOptions)
        {
            ArgumentNullException.ThrowIfNull(loggingOptions);

            _loggingOptions = loggingOptions.Value;
        }

        public async ValueTask Log(Level level, string message)
        {
            var timestamp = DateTimeOffset.UtcNow;

            var consoleMessage = string.Format(
                _loggingOptions.ConsoleLogFormat,
                timestamp.ToString(),
                level.ShortForm,
                message
            );

            Console.WriteLine(consoleMessage);

            if (_loggingOptions.LogToFile)
            {
                await LogToFile(level, message, timestamp);
            }
        }

        private static void RotateFile(string fullLogFilePath, string logFolder)
        {
            var rotatedFileExtension = DateTimeOffset.Now.ToString("yyyyMMddHHmmss");

            var rotatedLogFileName =
                $"{Path.GetFileNameWithoutExtension(fullLogFilePath)}.{rotatedFileExtension}";

            var rotatedLogFilePath = Path.Combine(logFolder, rotatedLogFileName);

            File.Move(fullLogFilePath, rotatedLogFilePath);
        }

        private static async ValueTask WriteLogMessageToFile(
            string fullLogFilePath,
            string fileMessage
        )
        {
            await using (
                var stream = new FileStream(
                    fullLogFilePath,
                    FileMode.Append,
                    FileAccess.Write,
                    FileShare.Read
                )
            )
            {
                await using (var writer = new StreamWriter(stream))
                {
                    await writer.WriteLineAsync(fileMessage);
                }
            }
        }

        private async ValueTask LogToFile(Level level, string message, DateTimeOffset timestamp)
        {
            var fileMessage = string.Format(
                _loggingOptions.FileLogFormat,
                timestamp,
                level.ShortForm,
                message
            );

            var fullLogFilePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                _loggingOptions.LogFilePath
            );

            var logFolder = Path.GetDirectoryName(fullLogFilePath);

            if (logFolder == null)
            {
                throw new InvalidOperationException(
                    "Could not determine the location of the logging folder."
                );
            }

            if (!Directory.Exists(logFolder))
            {
                Directory.CreateDirectory(logFolder);
            }

            await WriteLogMessageToFile(fullLogFilePath, fileMessage);

            if (ShouldRotateFile(fullLogFilePath))
            {
                RotateFile(fullLogFilePath, logFolder);
            }
        }

        private bool ShouldRotateFile(string fullLogFilePath)
        {
            var fileSize = new FileInfo(fullLogFilePath).Length;
            return fileSize > _loggingOptions.LogFileMaxSizeInBytes;
        }
    }
}
