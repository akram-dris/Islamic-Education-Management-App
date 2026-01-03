using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Allocations.GetAll;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.UnitTests.Features.Allocations.GetAll;

public class GetAllAllocationsQueryHandlerTests
{
    private readonly IAllocationRepository _allocationRepositoryMock;
    private readonly GetAllAllocationsQueryHandler _handler;

    public GetAllAllocationsQueryHandlerTests()
    {
        _allocationRepositoryMock = Substitute.For<IAllocationRepository>();
        _handler = new GetAllAllocationsQueryHandler(_allocationRepositoryMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedResponse_WhenAllocationsExist()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var classId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();
        
        var teacher = User.Create("teacher", "pass", "Teacher Name", QuranSchool.Domain.Enums.UserRole.Teacher).Value;
        var @class = Class.Create("Class Name").Value;
        var subject = Subject.Create("Subject Name").Value;

        var allocation = Allocation.Create(teacherId, classId, subjectId).Value;
        
        // Use reflection or a friendlier way if possible, but here we just need them to be not null
        // Since we are using NSubstitute and mocking the repository, we can return objects with properties set
        typeof(Allocation).GetProperty(nameof(Allocation.Teacher))?.SetValue(allocation, teacher);
        typeof(Allocation).GetProperty(nameof(Allocation.Class))?.SetValue(allocation, @class);
        typeof(Allocation).GetProperty(nameof(Allocation.Subject))?.SetValue(allocation, subject);

        var allocations = new List<Allocation> { allocation };
        
        _allocationRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(allocations);

        var query = new GetAllAllocationsQuery();

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var response = result.Value.First();
        response.Id.Should().Be(allocation.Id);
        response.TeacherName.Should().Be("Teacher Name");
        response.ClassName.Should().Be("Class Name");
        response.SubjectName.Should().Be("Subject Name");
        response.TeacherId.Should().Be(teacherId);
        response.ClassId.Should().Be(classId);
        response.SubjectId.Should().Be(subjectId);
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedResponse_WithNANames_WhenNavigationPropertiesAreNull()
    {
        // Arrange
        var allocation = Allocation.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()).Value;
        var allocations = new List<Allocation> { allocation };
        
        _allocationRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(allocations);

        var query = new GetAllAllocationsQuery();

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var response = result.Value.First();
        response.TeacherName.Should().Be("N/A");
        response.ClassName.Should().Be("N/A");
        response.SubjectName.Should().Be("N/A");
    }

    [Fact]
    public void AllocationResponse_ShouldSupportEquality()
    {
        var id = Guid.NewGuid();
        var tId = Guid.NewGuid();
        var cId = Guid.NewGuid();
        var sId = Guid.NewGuid();

        var res1 = new AllocationResponse(id, tId, "T", cId, "C", sId, "S");
        var res2 = new AllocationResponse(id, tId, "T", cId, "C", sId, "S");

        res1.Should().Be(res2);
        res1.ToString().Should().NotBeEmpty();
    }
}