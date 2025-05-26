using System.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;

namespace Sdc.LoggingService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            //builder.Services.AddAuthorization();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.Configure<LoggingOptions>(
                builder.Configuration.GetSection(LoggingOptions.ConfigurationSectionName)
            );

            builder.Services.Configure<LoggingWorkerOptions>(
                builder.Configuration.GetSection(LoggingWorkerOptions.ConfigurationSectionName)
            );

            builder.Services.AddSingleton<Logger>();
            builder.Services.AddSingleton<LogEntryCache>();
            builder.Services.AddHostedService<LoggingWorker>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            //app.UseHttpsRedirection();

            //app.UseAuthorization();

            app.MapGet(
                    "/logs",
                    async (
                        HttpContext httpContext,
                        LogEntryCache logEntryCache,
                        Logger logger,
                        int? limit
                    ) =>
                    {
                        await logger.Log(
                            Levels.Information,
                            $"Executing {httpContext.Request.Path}"
                        );

                        var stopWatch = Stopwatch.StartNew();

                        var realLimit = limit ?? 5;

                        if (realLimit is < 1 or > 100)
                        {
                            return Results.BadRequest("Query parameter 'limit' is out of range.");
                        }

                        var logEntries = logEntryCache.GetEntries(realLimit);

                        await logger.Log(
                            Levels.Information,
                            $"Executed {httpContext.Request.Path} in {stopWatch.Elapsed.TotalMilliseconds}ms."
                        );

                        return Results.Ok(logEntries);
                    }
                )
                .WithName("GetRecentLogs");

            app.Run();
        }
    }
}
