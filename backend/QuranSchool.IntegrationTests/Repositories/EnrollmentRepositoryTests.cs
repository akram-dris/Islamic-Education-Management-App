using FluentAssertions;
using Xunit;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Enums;
using QuranSchool.Infrastructure.Repositories;
using QuranSchool.IntegrationTests.Abstractions;

namespace QuranSchool.IntegrationTests.Repositories;

public class EnrollmentRepositoryTests : BaseIntegrationTest
{
    [Fact]
    public async Task AddAsync_ShouldSaveEnrollment()
    {
        var repo = new EnrollmentRepository(DbContext);
        var student = await CreateStudent();
        var @class = await CreateClass();
        var enrollment = Enrollment.Create(student.Id, @class.Id).Value;

        await repo.AddAsync(enrollment);

        var result = await repo.GetByIdAsync(enrollment.Id);
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByStudentIdAsync_ShouldReturnEnrollments()
    {
        var repo = new EnrollmentRepository(DbContext);
        var student = await CreateStudent();
        var @class = await CreateClass();
        var enrollment = Enrollment.Create(student.Id, @class.Id).Value;
        await repo.AddAsync(enrollment);

        var result = await repo.GetByStudentIdAsync(student.Id);

        result.Should().Contain(e => e.Id == enrollment.Id);
    }

    private async Task<User> CreateStudent()
    {
        var user = User.Create(Guid.NewGuid().ToString(), "h", "S", UserRole.Student).Value;
        DbContext.Set<User>().Add(user);
        await DbContext.SaveChangesAsync();
        return user;
    }

    private async Task<Class> CreateClass()
    {
        var @class = Class.Create(Guid.NewGuid().ToString()).Value;
        DbContext.Set<Class>().Add(@class);
        await DbContext.SaveChangesAsync();
        return @class;
    }
}
