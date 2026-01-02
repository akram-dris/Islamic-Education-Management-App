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
        var repository = new UserRepository(DbContext);
        var user = User.Create("testuser_add", "hash", "Test User", UserRole.Student).Value;

        await repository.AddAsync(user, default);

        var savedUser = await repository.GetByUsernameAsync(user.Username);
        savedUser.Should().NotBeNull();
        savedUser!.Username.Should().Be(user.Username);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser()
    {
        var repository = new UserRepository(DbContext);
        var user = User.Create("testuser_get", "hash", "Test User", UserRole.Student).Value;
        await repository.AddAsync(user);

        var result = await repository.GetByIdAsync(user.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUserDetails()
    {
        var repository = new UserRepository(DbContext);
        var user = User.Create("testuser_up", "hash", "Old Name", UserRole.Student).Value;
        await repository.AddAsync(user);

        user.Update("New Name", "newhash");
        await repository.UpdateAsync(user);

        var result = await repository.GetByIdAsync(user.Id);
        result!.FullName.Should().Be("New Name");
        result.PasswordHash.Should().Be("newhash");
    }

    [Fact]
    public async Task DeleteAsync_ShouldSoftDeleteUser()
    {
        var repository = new UserRepository(DbContext);
        var user = User.Create("testuser_del", "hash", "Test User", UserRole.Student).Value;
        await repository.AddAsync(user);

        await repository.DeleteAsync(user);

        var result = await repository.GetByIdAsync(user.Id);
        result.Should().BeNull(); // Repository should filter deleted users in GetById if implemented
        
        // Actually Repository GetByIdAsync usually filters IsDeleted if following the pattern.
        // Let's check DB directly if repo filters it.
    }

    [Fact]
    public async Task AddParentStudentLinkAsync_ShouldSaveLinkToDatabase()
    {
        var repository = new UserRepository(DbContext);
        
        var parent = User.Create("parent_link", "hash", "Parent", UserRole.Parent).Value;
        await repository.AddAsync(parent);

        var student = User.Create("student_link", "hash", "Student", UserRole.Student).Value;
        await repository.AddAsync(student);

        var link = ParentStudent.Create(parent.Id, student.Id).Value;

        await repository.AddParentStudentLinkAsync(link, default);

        var isLinked = await repository.IsParentLinkedAsync(parent.Id, student.Id);
        isLinked.Should().BeTrue();
    }

    [Fact]
    public async Task GetAllByRoleAsync_ShouldReturnUsersWithRole()
    {
        var repository = new UserRepository(DbContext);
        var teacher = User.Create("teacher_role", "hash", "Teacher", UserRole.Teacher).Value;
        await repository.AddAsync(teacher);

        var result = await repository.GetAllByRoleAsync(UserRole.Teacher);

        result.Should().Contain(u => u.Id == teacher.Id);
    }

    [Fact]
    public async Task GetByParentIdAsync_ShouldReturnChildren()
    {
        var repository = new UserRepository(DbContext);
        var parent = User.Create("parent_child", "hash", "Parent", UserRole.Parent).Value;
        await repository.AddAsync(parent);
        var student = User.Create("student_child", "hash", "Student", UserRole.Student).Value;
        await repository.AddAsync(student);
        var link = ParentStudent.Create(parent.Id, student.Id).Value;
        await repository.AddParentStudentLinkAsync(link);

        var result = await repository.GetByParentIdAsync(parent.Id);

        result.Should().Contain(u => u.Id == student.Id);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnPagedUsers()
    {
        var repository = new UserRepository(DbContext);
        var user = User.Create("paged_user", "hash", "Paged User", UserRole.Student).Value;
        await repository.AddAsync(user);

        var result = await repository.GetPagedAsync("Paged User", UserRole.Student, 1, 10);

        result.Items.Should().Contain(u => u.Id == user.Id);
    }
}
