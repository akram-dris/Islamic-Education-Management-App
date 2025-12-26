using Microsoft.EntityFrameworkCore;
using QuranSchool.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace QuranSchool.IntegrationTests.Abstractions;

public abstract class BaseIntegrationTest : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:17-alpine")
        .WithDatabase("QuranSchool")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    protected ApplicationDbContext DbContext { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_dbContainer.GetConnectionString());

        DbContext = new ApplicationDbContext(optionsBuilder.Options);
        await DbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await DbContext.DisposeAsync();
    }
}
