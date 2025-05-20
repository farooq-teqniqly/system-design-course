namespace LoggerService;

public interface ITimeService
{
    DateTimeOffset UtcNow { get; }
}