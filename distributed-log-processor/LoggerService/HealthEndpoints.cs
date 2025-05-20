namespace LoggerService;

public static class HealthEndpoints
{
    public static void MapEndpoints(WebApplication app)
    {
        app.MapGet(
            "/health",
            (HttpContext _, ITimeService timeService) =>
                Results.Ok(new { status = "healthy", utc = timeService.UtcNow })
        );
    }
}
