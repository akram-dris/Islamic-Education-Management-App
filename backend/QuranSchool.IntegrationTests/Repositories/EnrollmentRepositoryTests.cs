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

        var student = new User
        {
            Id = Guid.NewGuid(),
            Username = "enrolled_student",
            FullName = "Enrolled Student",
            PasswordHash = "hash",
            Role = UserRole.Student,
            CreatedAt = DateTime.UtcNow
        };
        await userRepo.AddAsync(student);

        var schoolClass = new Class
        {
            Id = Guid.NewGuid(),
            Name = "Enrollment Test Class"
        };
        await classRepo.AddAsync(schoolClass);

        var enrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            StudentId = student.Id,
            ClassId = schoolClass.Id
        };

        // Act
        await enrollmentRepo.AddAsync(enrollment, default);

        // Assert
        var exists = await enrollmentRepo.ExistsAsync(student.Id, schoolClass.Id);
        exists.Should().BeTrue();
    }
}