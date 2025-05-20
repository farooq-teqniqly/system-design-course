using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LoggerService.IO;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace LoggerService.IntegrationTests
{
    public class LoggerEndpointsTests : IntegrationTestBase
    {
        public LoggerEndpointsTests(WebApplicationFactory<Program> factory)
            : base(factory) { }

        [Fact]
        public async Task TailLogs_Returns_BadRequest_On_BadRequestResult()
        {
            var error = "Invalid limit";
            var logTailResult = LogTailResult.BadRequest(error);

            var client = CreateClientWithServices(services =>
            {
                RemoveService<ILogFileReader>(services);

                var mockLogFileReader = Substitute.For<ILogFileReader>();
                mockLogFileReader.GetLatestLogsAsync(Arg.Any<int?>()).Returns(logTailResult);
                services.AddSingleton(mockLogFileReader);
            });

            var response = await client.GetAsync("/logs/tail?limit=-1");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var json = await response.Content.ReadAsStringAsync();
            json.Should().Contain(error);
        }

        [Fact]
        public async Task TailLogs_Returns_InternalServerError_On_Exception()
        {
            var client = CreateClientWithServices(services =>
            {
                RemoveService<ILogFileReader>(services);

                var mockLogFileReader = Substitute.For<ILogFileReader>();
                mockLogFileReader
                    .GetLatestLogsAsync(Arg.Any<int?>())
                    .Throws(new Exception("Unexpected"));
                services.AddSingleton(mockLogFileReader);
            });

            var response = await client.GetAsync("/logs/tail");

            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

            var json = await response.Content.ReadAsStringAsync();
            json.Should().Contain("An unexpected error occurred");
        }

        [Fact]
        public async Task TailLogs_Returns_Ok_With_Logs()
        {
            var expectedLogs = new List<Dictionary<string, string>>
            {
                new() { ["level"] = "info", ["message"] = "Test log" },
            };

            var logTailResult = LogTailResult.Ok(expectedLogs);

            var client = CreateClientWithServices(services =>
            {
                RemoveService<ILogFileReader>(services);

                var mockLogFileReader = Substitute.For<ILogFileReader>();
                mockLogFileReader.GetLatestLogsAsync(Arg.Any<int?>()).Returns(logTailResult);
                services.AddSingleton(mockLogFileReader);
            });

            var response = await client.GetAsync("/logs/tail?limit=1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var logs = await response.Content.ReadFromJsonAsync<List<Dictionary<string, string>>>();
            logs.Should().BeEquivalentTo(expectedLogs);
        }

        [Fact]
        public async Task TailLogs_Returns_Problem_On_ServerError()
        {
            var error = "Server error";
            var logTailResult = CreateServerErrorLogTailResult(error);

            var client = CreateClientWithServices(services =>
            {
                RemoveService<ILogFileReader>(services);

                var mockLogFileReader = Substitute.For<ILogFileReader>();
                mockLogFileReader.GetLatestLogsAsync(Arg.Any<int?>()).Returns(logTailResult);
                services.AddSingleton(mockLogFileReader);
            });

            var response = await client.GetAsync("/logs/tail");

            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

            var json = await response.Content.ReadAsStringAsync();
            json.Should().Contain(error);
        }

        private static LogTailResult CreateServerErrorLogTailResult(string error)
        {
            var ctor = typeof(LogTailResult)
                .GetConstructors(
                    System.Reflection.BindingFlags.Instance
                        | System.Reflection.BindingFlags.NonPublic
                        | System.Reflection.BindingFlags.Public
                )
                .FirstOrDefault(c =>
                {
                    var parameters = c.GetParameters();
                    return parameters.Length == 3
                        && parameters[0].ParameterType == typeof(List<Dictionary<string, string>>)
                        && parameters[1].ParameterType == typeof(string)
                        && parameters[2].ParameterType == typeof(bool);
                });

            return (LogTailResult)ctor!.Invoke([null, error, true]);
        }
    }
}
