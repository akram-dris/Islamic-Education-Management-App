using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Authentication;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Assignments.GetMy;
using QuranSchool.Domain.Entities;

namespace QuranSchool.UnitTests.Features.Assignments.GetMy;

public class GetMyAssignmentsQueryHandlerTests
{
    private readonly IAssignmentRepository _assignmentRepositoryMock;
    private readonly IEnrollmentRepository _enrollmentRepositoryMock;
    private readonly GetMyAssignmentsQueryHandler _handler;

    public GetMyAssignmentsQueryHandlerTests()
    {
        _assignmentRepositoryMock = Substitute.For<IAssignmentRepository>();
        _enrollmentRepositoryMock = Substitute.For<IEnrollmentRepository>();
        _handler = new GetMyAssignmentsQueryHandler(_assignmentRepositoryMock, _enrollmentRepositoryMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnAssignments_ForCurrentUser()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var classId = Guid.NewGuid();
        
        var enrollment = Enrollment.Create(studentId, classId).Value;
        var assignment = Assignment.Create(Guid.NewGuid(), "Title", "Desc", DateOnly.FromDateTime(DateTime.Now)).Value;
        
        _enrollmentRepositoryMock.GetByStudentIdAsync(studentId, default).Returns(new List<Enrollment> { enrollment });
        _assignmentRepositoryMock.GetByClassIdAsync(classId, default).Returns(new List<Assignment> { assignment });

        // Act
        var result = await _handler.Handle(new GetMyAssignmentsQuery(studentId), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].Title.Should().Be("Title");
    }
}
