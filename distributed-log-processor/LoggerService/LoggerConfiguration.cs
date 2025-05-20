namespace LoggerService;

public class LoggerConfiguration
{
    private readonly IConfiguration _config;

    public LoggerConfiguration(IConfiguration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));

        LogsDirectory = GetLogsDirectory();
        LogFilePattern = GetLogFilePattern();
    }

    public string LogFilePattern { get; }
    public string LogsDirectory { get; }

    private string GetLogFilePattern()
    {
        var path = _config
            .GetSection("Serilog:WriteTo")
            .GetChildren()
            .FirstOrDefault(x => x["Name"] == "File")
            ?["Args:path"];

        if (string.IsNullOrWhiteSpace(path))
        {
            return "log*.json";
        }

        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
        var extension = Path.GetExtension(path);

        if (string.IsNullOrWhiteSpace(extension) || extension.Length < 2)
        {
            return $"{fileNameWithoutExtension}*";
        }

        extension = extension[1..];

        return $"{fileNameWithoutExtension}*.{extension}";
    }

    private string GetLogsDirectory()
    {
        var logPath = _config
            .GetSection("Serilog:WriteTo")
            .GetChildren()
            .FirstOrDefault(x => x["Name"] == "File")
            ?["Args:path"];

        if (string.IsNullOrEmpty(logPath))
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "logs");
        }

        var dir = Path.GetDirectoryName(logPath);

        return string.IsNullOrEmpty(dir)
            ? Directory.GetCurrentDirectory()
            : Path.GetFullPath(dir, Directory.GetCurrentDirectory());
    }
}
