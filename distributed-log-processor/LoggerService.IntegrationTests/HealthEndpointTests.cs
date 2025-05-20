using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace LoggerService.IntegrationTests
{
    namespace LoggerService.IntegrationTests
    {
        public class HealthEndpointTests : IntegrationTestBase
        {
            public HealthEndpointTests(WebApplicationFactory<Program> factory)
                : base(factory) { }

            [Fact]
            public async Task Health_Returns_Healthy()
            {
                var expectedTimestamp = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
                var expectedStatus = "healthy";

                var client = CreateClientWithServices(services =>
                {
                    RemoveService<ILogFileReader>(services);
                    RemoveService<ITimeService>(services);

                    services.Configure<LogTailOptions>(opts => opts.Limit = 10);
                    var mockLogFileReader = Substitute.For<ILogFileReader>();
                    services.AddSingleton(mockLogFileReader);

                    var mockTimeService = Substitute.For<ITimeService>();
                    mockTimeService.UtcNow.Returns(expectedTimestamp);
                    services.AddSingleton(mockTimeService);
                });

                var response = await client.GetAsync("/health");

                response.IsSuccessStatusCode.Should().BeTrue();

                var json = await response.Content.ReadAsStringAsync();

                using var doc = System.Text.Json.JsonDocument.Parse(json);
                var root = doc.RootElement;

                root.GetProperty("status").GetString().Should().Be(expectedStatus);
                root.GetProperty("utc").GetDateTimeOffset().Should().Be(expectedTimestamp);
            }
        }
    }
}
