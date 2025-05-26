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
        }
    }
}
