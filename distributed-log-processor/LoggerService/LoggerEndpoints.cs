using Serilog;

namespace LoggerService
{
    public static class LoggerEndpoints
    {
        public static void MapEndpoints(WebApplication app)
        {
            app.MapGet(
                "/logs/tail",
                async (int? limit, ILogFileReader logFileReader) =>
                {
                    try
                    {
                        var result = await logFileReader.GetLatestLogsAsync(limit);

                        if (!result.Success)
                        {
                            return result.IsServerError
                                ? Results.Problem(result.Error, statusCode: 500)
                                : Results.BadRequest(new { error = result.Error });
                        }

                        return Results.Ok(result.Logs);
                    }
                    catch (Exception exception)
                    {
                        Log.Logger.Error(
                            exception,
                            "An unexpected error occurred while getting log files."
                        );

                        return Results.InternalServerError("An unexpected error occurred.");
                    }
                }
            );
        }
    }
}
