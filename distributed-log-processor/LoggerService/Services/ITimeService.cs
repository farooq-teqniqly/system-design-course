namespace LoggerService.Services;

public interface ITimeService
{
    DateTimeOffset UtcNow { get; }
}