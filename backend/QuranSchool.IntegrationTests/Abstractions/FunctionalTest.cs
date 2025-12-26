using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuranSchool.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace QuranSchool.IntegrationTests.Abstractions;

public abstract class FunctionalTest : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:17-alpine")
        .WithDatabase("QuranSchool")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    protected HttpClient HttpClient { get; private set; } = default!;
    protected IServiceProvider ServiceProvider { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        // Environment variables should be provided by the test runner or .env
        await _dbContainer.StartAsync();

        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["JWT_SECRET_KEY"] = "super-secret-key-that-is-at-least-32-characters-long",
                        ["JWT_ISSUER"] = "QuranSchool",
                        ["JWT_AUDIENCE"] = "QuranSchool"
                    });
                });

                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseNpgsql(_dbContainer.GetConnectionString()));
                });
            });

        HttpClient = factory.CreateClient();
        ServiceProvider = factory.Services;

        using var scope = ServiceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}
