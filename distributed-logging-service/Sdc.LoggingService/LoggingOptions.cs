using System.ComponentModel.DataAnnotations;

namespace Sdc.LoggingService
{
    public class LoggingOptions
    {
        public const string ConfigurationSectionName = nameof(LoggingOptions);

        public string ConsoleLogFormat { get; set; } = "[{0}] [{1}] {2}";
        public string FileLogFormat { get; set; } = "{0} | {1} | {2}";

        [Range(
            1024,
            1024 * 1000 * 100,
            ErrorMessage = "Value for {0} must be between {1} and {2}."
        )]
        public int LogFileMaxSizeInBytes { get; set; } = 1024 * 1000;
        public string LogFilePath { get; set; } = "logs/logger.log";
        public bool LogToFile { get; set; } = false;
    }
}
