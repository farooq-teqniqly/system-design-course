using System.ComponentModel.DataAnnotations;

namespace Sdc.LoggingService;

public class LoggingWorkerOptions
{
    public const string ConfigurationSectionName = nameof(LoggingWorkerOptions);

    [Range(1, 30, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int FrequencyInSeconds { get; set; } = 1;
}
