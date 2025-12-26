using FluentAssertions;
using Xunit;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Enums;
using QuranSchool.Infrastructure.Repositories;
using QuranSchool.IntegrationTests.Abstractions;

namespace QuranSchool.IntegrationTests.Repositories;

public class UserRepositoryTests : BaseIntegrationTest
{
    [Fact]
    public async Task AddAsync_ShouldSaveUserToDatabase()
    {
        // Arrange
        var repository = new UserRepository(DbContext);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            FullName = "Test User",
            PasswordHash = "hash",
            Role = UserRole.Student,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await repository.AddAsync(user, default);

        // Assert
        var savedUser = await repository.GetByUsernameAsync(user.Username);
        savedUser.Should().NotBeNull();
        savedUser!.Username.Should().Be(user.Username);
    }

    [Fact]
    public async Task AddParentStudentLinkAsync_ShouldSaveLinkToDatabase()
    {
        // Arrange
        var repository = new UserRepository(DbContext);
        
        var parent = new User
        {
            Id = Guid.NewGuid(),
            Username = "parent_link",
            FullName = "Parent",
            PasswordHash = "hash",
            Role = UserRole.Parent,
            CreatedAt = DateTime.UtcNow
        };
        await repository.AddAsync(parent);

        var student = new User
        {
            Id = Guid.NewGuid(),
            Username = "student_link",
            FullName = "Student",
            PasswordHash = "hash",
            Role = UserRole.Student,
            CreatedAt = DateTime.UtcNow
        };
        await repository.AddAsync(student);

        var link = new ParentStudent
        {
            ParentId = parent.Id,
            StudentId = student.Id
        };

        // Act
        await repository.AddParentStudentLinkAsync(link, default);

        // Assert
        var isLinked = await repository.IsParentLinkedAsync(parent.Id, student.Id);
        isLinked.Should().BeTrue();
    }
}