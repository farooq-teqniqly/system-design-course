namespace LoggerService
{
    public class LogTailOptions
    {
        public const string SectionName = "LogTail";
        public int Limit { get; set; } = 100;

        public int ClampLimit(int? requested)
        {
            return requested is null or < 1 or > 100 ? Limit : requested.Value;
        }
    }
}
