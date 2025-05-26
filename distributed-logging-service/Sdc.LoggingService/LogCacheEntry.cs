namespace Sdc.LoggingService;

public sealed record LogCacheEntry(Level Level, DateTimeOffset Timestamp, string Message);
