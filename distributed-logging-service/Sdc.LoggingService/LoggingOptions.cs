namespace Sdc.LoggingService
{
    public class LoggingOptions
    {
        public const string ConfigurationSectionName = nameof(LoggingOptions);

        public string ConsoleLogFormat { get; set; } = "[{0}] [{1}] {2}";
        public string FileLogFormat { get; set; } = "{0} | {1} | {2}";
        public string LogFilePath { get; set; } = "logs/logger.log";
        public bool LogToFile { get; set; } = false;
    }
}
