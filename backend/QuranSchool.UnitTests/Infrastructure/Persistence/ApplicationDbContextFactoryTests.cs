using FluentAssertions;
using QuranSchool.Infrastructure.Persistence;
using Xunit;

namespace QuranSchool.UnitTests.Infrastructure.Persistence;

public class ApplicationDbContextFactoryTests
{
    [Fact]
    public void CreateDbContext_ShouldReturnContext()
    {
        // Arrange
        var factory = new ApplicationDbContextFactory();

        // Act
        var context = factory.CreateDbContext(Array.Empty<string>());

        // Assert
        context.Should().NotBeNull();
        context.Database.Should().NotBeNull();
    }
}
