using Microsoft.Extensions.Options;

namespace Sdc.LoggingService
{
    public sealed class LoggingWorker : BackgroundService
    {
        private readonly LoggingWorkerOptions _options;
        private readonly IServiceProvider _serviceProvider;

        public LoggingWorker(
            IServiceProvider serviceProvider,
            IOptions<LoggingWorkerOptions> options
        )
        {
            _serviceProvider =
                serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            ArgumentNullException.ThrowIfNull(options);

            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await using (var scope = _serviceProvider.CreateAsyncScope())
                {
                    var logger = scope.ServiceProvider.GetRequiredService<Logger>();
                    var call = LogMessages.PickRandom();

                    await call(logger);

                    await Task.Delay(
                        TimeSpan.FromSeconds(_options.FrequencyInSeconds),
                        stoppingToken
                    );
                }
            }
        }
    }
}
