namespace Sdc.LoggingService;

public static class LogMessages
{
    private static readonly List<Func<Logger, Task>> _calls =
    [
        async (l) => await l.Log(Levels.Information, "Service started successfully."),
        async (l) => await l.Log(Levels.Warning, "Configuration file not found, using defaults."),
        async (l) => await l.Log(Levels.Error, "Failed to connect to the database."),
        async (l) => await l.Log(Levels.Information, "User 'admin' logged in."),
        async (l) => await l.Log(Levels.Debug, "Cache miss for key 'session_123'."),
        async (l) => await l.Log(Levels.Information, "Scheduled job executed at midnight."),
        async (l) => await l.Log(Levels.Warning, "Disk space running low on /dev/sda1."),
        async (l) => await l.Log(Levels.Error, "Unhandled exception occurred in worker thread."),
        async (l) => await l.Log(Levels.Information, "Processed 150 records in 2.3 seconds."),
        async (l) =>
            await l.Log(
                Levels.Debug,
                "Request headers: { \"User-Agent\": \"PostmanRuntime/7.28.0\" }"
            ),
        async (l) =>
            await l.Log(Levels.Critical, "Critical error: Out of memory. Shutting down service."),
    ];

    private static readonly Random _random = new(123);

    public static Func<Logger, Task> PickRandom()
    {
        var index = _random.Next(_calls.Count);
        return _calls[index];
    }
}
