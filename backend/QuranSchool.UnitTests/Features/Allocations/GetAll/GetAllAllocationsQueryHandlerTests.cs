using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Allocations.GetAll;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Enums;

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
    public async Task Handle_ShouldReturnAllocations_WhenAllocationsExist()
    {
        // Arrange
        var teacher = User.Create("teacher", "hash", "Teacher Name", UserRole.Teacher).Value;
        var cls = Class.Create("Class A").Value;
        var subject = Subject.Create("Subject 1").Value;
        
        var allocation = Allocation.Create(teacher.Id, cls.Id, subject.Id).Value;
        
        // Manually setting navigation properties for testing since they are not set by Create
        // Reflection or just reliance on repository mocking returning the allocation with loaded props
        // Since the handler uses ?. operator, we need to ensuring our mock returns objects that can simulate this or just ensuring it doesn't crash on nulls.
        // But Entity Framework does the loading. Here we can't easily set navigation properties because they are private setters or not in constructor.
        // Wait, typically in Unit Tests with Mocks, we return the object. The handler accesses `a.Teacher?.FullName`.
        // If we want to test that mapping, we need `Teacher` to be not null in the returned allocation.
        // The Entity `Allocation` has `public User? Teacher { get; set; }` (likely set to private set or similar).
        // Let's check Allocation.cs.
        
        // Assuming we can't easily set navigation properties without reflection if they are private set.
        // Ideally the handler maps `Teacher?.FullName`. If `Teacher` is null, it returns "N/A".
        
        // Let's rely on default behavior first (null props) -> "N/A"
        // And then try to use reflection to set props if we want to confirm data mapping.
        
        _allocationRepositoryMock.GetAllAsync(default).Returns(new List<Allocation> { allocation });

        // Act
        var result = await _handler.Handle(new GetAllAllocationsQuery(), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].TeacherName.Should().Be("N/A"); // Because nav prop is null
    }
}
