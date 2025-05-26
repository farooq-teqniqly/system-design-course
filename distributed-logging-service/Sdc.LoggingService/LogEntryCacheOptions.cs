using System.ComponentModel.DataAnnotations;

namespace Sdc.LoggingService;

public sealed class LogEntryCacheOptions
{
    [Range(1, 1000, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int MaxSize { get; set; } = 100;
}
