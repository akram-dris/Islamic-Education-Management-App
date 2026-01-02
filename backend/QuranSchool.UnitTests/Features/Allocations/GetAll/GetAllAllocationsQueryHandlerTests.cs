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
    public async Task Handle_ShouldReturnList()
    {
        var query = new GetAllAllocationsQuery();
        var allocations = new List<Allocation>
        {
            Allocation.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()).Value
        };
        
        _allocationRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(allocations);

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(1);
    }
}