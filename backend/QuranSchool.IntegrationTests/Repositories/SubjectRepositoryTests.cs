using FluentAssertions;
using Xunit;
using QuranSchool.Domain.Entities;
using QuranSchool.Infrastructure.Repositories;
using QuranSchool.IntegrationTests.Abstractions;

namespace QuranSchool.IntegrationTests.Repositories;

public class SubjectRepositoryTests : BaseIntegrationTest
{
    [Fact]
    public async Task AddAsync_ShouldSaveSubjectToDatabase()
    {
        // Arrange
        var repository = new SubjectRepository(DbContext);
        var subject = Subject.Create("Integration Test Subject").Value;

        // Act
        await repository.AddAsync(subject, default);

        // Assert
        var exists = await repository.ExistsAsync(subject.Name);
        exists.Should().BeTrue();
    }
}
