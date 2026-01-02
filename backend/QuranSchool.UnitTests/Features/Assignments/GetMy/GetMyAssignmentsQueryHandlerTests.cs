using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Assignments.GetMy;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Abstractions;

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
    public async Task Handle_ShouldReturnAssignmentsForUser()
    {
        var userId = Guid.NewGuid();
        var query = new GetMyAssignmentsQuery(userId);
        
        var enrollment = Enrollment.Create(userId, Guid.NewGuid()).Value;
        var assignments = new List<Assignment>
        {
            Assignment.Create(enrollment.ClassId, "T", "D", DateOnly.MaxValue).Value
        };
        
        _enrollmentRepositoryMock.GetByStudentIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(new List<Enrollment> { enrollment });
            
        _assignmentRepositoryMock.GetByClassIdAsync(enrollment.ClassId, Arg.Any<CancellationToken>())
            .Returns(assignments);

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(1);
    }
}