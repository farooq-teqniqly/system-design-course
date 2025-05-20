using Serilog;

namespace LoggerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHostedService<Worker>();

            builder.Services.Configure<LogTailOptions>(
                builder.Configuration.GetSection(LogTailOptions.SectionName)
            );

            builder.Services.AddSingleton<LoggerConfiguration>(_ => new LoggerConfiguration(
                builder.Configuration
            ));

            builder.Services.AddSingleton<ILogFileReader, LogFileReader>();
            builder.Services.AddSingleton<ITimeService, TimeService>();

            ConfigureSerilog(builder);

            var app = builder.Build();

            HealthEndpoints.MapEndpoints(app);
            LoggerEndpoints.MapEndpoints(app);

            app.Run();
        }

        private static void ConfigureSerilog(WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();

            var isDevelopment = string.Equals(
                builder.Environment.EnvironmentName.ToLower(),
                "development",
                StringComparison.InvariantCultureIgnoreCase
            );

            var loggingConfig = new Serilog.LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .WriteTo.Conditional(_ => isDevelopment, c => c.Trace());

            Log.Logger = loggingConfig.CreateLogger();
            builder.Logging.AddSerilog();
        }
    }
}
