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
    public async Task AddAsync_ShouldSaveEnrollmentToDatabase()
    {
        // Arrange
        var enrollmentRepo = new EnrollmentRepository(DbContext);
        var userRepo = new UserRepository(DbContext);
        var classRepo = new ClassRepository(DbContext);

        var student = User.Create("enrolled_student", "hash", "Enrolled Student", UserRole.Student).Value;
        await userRepo.AddAsync(student);

        var schoolClass = Class.Create("Enrollment Test Class").Value;
        await classRepo.AddAsync(schoolClass);

        var enrollment = Enrollment.Create(student.Id, schoolClass.Id).Value;

        // Act
        await enrollmentRepo.AddAsync(enrollment, default);

        // Assert
        var exists = await enrollmentRepo.ExistsAsync(student.Id, schoolClass.Id);
        exists.Should().BeTrue();
    }
}