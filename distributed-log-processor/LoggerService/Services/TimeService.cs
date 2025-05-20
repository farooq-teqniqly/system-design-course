namespace LoggerService.Services;

public sealed class TimeService : ITimeService
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
