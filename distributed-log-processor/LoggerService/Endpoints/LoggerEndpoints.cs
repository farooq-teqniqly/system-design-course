using LoggerService.IO;
using LoggerService.Models;
using Serilog;

namespace LoggerService.Endpoints
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

            app.MapPost(
                "/logs/submit",
                (ILogWriter logWriter, LogEntryDto entry) =>
                {
                    try
                    {
                        var logger = Log
                            .Logger.ForContext("SourceContext", entry.SourceContext ?? "External")
                            .ForContext("ApplicationName", entry.ApplicationName ?? "External");

                        if (entry.Properties != null)
                        {
                            foreach (var prop in entry.Properties)
                            {
                                object? value = prop.Value;

                                value = ConvertJsonElement(value);

                                logger = logger.ForContext(
                                    prop.Key,
                                    value,
                                    destructureObjects: true
                                );
                            }
                        }

                        logWriter.LogByLevel(entry, logger);

                        return Results.Ok(new { status = "logged" });
                    }
                    catch (Exception ex)
                    {
                        Log.Logger.Error(ex, "Failed to submit log entry.");
                        return Results.Problem("Failed to submit log entry.", statusCode: 500);
                    }
                }
            );
        }

        private static object? ConvertJsonElement(object value)
        {
            if (value is not System.Text.Json.JsonElement jsonElement)
            {
                return value;
            }

            value = jsonElement.ValueKind switch
            {
                System.Text.Json.JsonValueKind.String => jsonElement.GetString() ?? string.Empty,
                System.Text.Json.JsonValueKind.Number => jsonElement.TryGetInt64(out var l)
                    ? l
                    : (double?)jsonElement.GetDouble(),
                System.Text.Json.JsonValueKind.True => (bool?)true,
                System.Text.Json.JsonValueKind.False => (bool?)false,
                System.Text.Json.JsonValueKind.Object or System.Text.Json.JsonValueKind.Array =>
                    jsonElement.ToString(),
                _ => null!,
            };
            return value;
        }
    }
}
