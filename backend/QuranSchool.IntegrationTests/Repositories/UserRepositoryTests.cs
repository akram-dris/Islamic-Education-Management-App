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
        var user = User.Create("testuser", "hash", "Test User", UserRole.Student).Value;

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
        
        var parent = User.Create("parent_link", "hash", "Parent", UserRole.Parent).Value;
        await repository.AddAsync(parent);

        var student = User.Create("student_link", "hash", "Student", UserRole.Student).Value;
        await repository.AddAsync(student);

        var link = ParentStudent.Create(parent.Id, student.Id).Value;

        // Act
        await repository.AddParentStudentLinkAsync(link, default);

        // Assert
        var isLinked = await repository.IsParentLinkedAsync(parent.Id, student.Id);
        isLinked.Should().BeTrue();
    }
}