namespace LoggerService.Models
{
    public class LogEntryDto
    {
        public string? ApplicationName { get; set; }
        public string? Exception { get; set; }
        public string Level { get; set; } = "Information";
        public string Message { get; set; } = string.Empty;

        public Dictionary<string, object>? Properties { get; set; }
        public string? SourceContext { get; set; }
    }
}
