using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace LoggerService.IntegrationTests;

public abstract class IntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly WebApplicationFactory<Program> _factory;

    protected IntegrationTestBase(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    protected HttpClient CreateClientWithServices(Action<IServiceCollection> configureServices)
    {
        var factory = WithMockedServices(configureServices);
        return factory.CreateClient();
    }

    protected void RemoveService<T>(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(T));
        if (descriptor != null)
            services.Remove(descriptor);
    }

    protected WebApplicationFactory<Program> WithMockedServices(
        Action<IServiceCollection> configureServices
    )
    {
        return _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(configureServices);
        });
    }
}
