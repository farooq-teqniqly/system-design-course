using System.Diagnostics;

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
            builder.Services.AddHostedService<LoggingWorker>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            //app.UseHttpsRedirection();

            //app.UseAuthorization();

            var summaries = new[]
            {
                "Freezing",
                "Bracing",
                "Chilly",
                "Cool",
                "Mild",
                "Warm",
                "Balmy",
                "Hot",
                "Sweltering",
                "Scorching",
            };

            app.MapGet(
                    "/weatherforecast",
                    async (HttpContext httpContext, Logger logger) =>
                    {
                        await logger.Log(
                            Levels.Information,
                            $"Executing {httpContext.Request.Path}"
                        );

                        var stopWatch = Stopwatch.StartNew();

                        var forecast = Enumerable
                            .Range(1, 5)
                            .Select(index => new WeatherForecast
                            {
                                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                                TemperatureC = Random.Shared.Next(-20, 55),
                                Summary = summaries[Random.Shared.Next(summaries.Length)],
                            })
                            .ToArray();

                        await logger.Log(
                            Levels.Information,
                            $"Executed {httpContext.Request.Path} in {stopWatch.Elapsed.TotalMilliseconds}ms."
                        );

                        return forecast;
                    }
                )
                .WithName("GetWeatherForecast");

            app.Run();
        }
    }
}
