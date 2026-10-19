using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;

namespace DoseLab.ApiTests.Harness;

/// <summary>
/// Lesson 11: a custom WebApplicationFactory. It starts the real API in memory, but:
/// no simulated database delay, a fake clock the test controls, and its own API key.
/// Each test class that uses it as an IClassFixture gets its own API with fresh data.
/// </summary>
public sealed class DoseLabApiFactory : WebApplicationFactory<Program>
{
    public const string ApiKey = "test-key";

    public FakeTimeProvider Clock { get; } = new(new DateTimeOffset(2026, 3, 1, 9, 0, 0, TimeSpan.Zero));

    public HttpClient CreateClientWithApiKey()
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Add("X-Api-Key", ApiKey);
        return client;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("Lab:SimulatedLatencyMs", "0");
        builder.UseSetting("Lab:ApiKey", ApiKey);

        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<TimeProvider>(Clock);
        });
    }
}
