using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Classes.GetAll;
using QuranSchool.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace QuranSchool.UnitTests.Features.Classes.GetAll;

public class GetAllClassesQueryHandlerTests
{
    private readonly IClassRepository _classRepositoryMock;
    private readonly IMemoryCache _memoryCacheMock;
    private readonly GetAllClassesQueryHandler _handler;

    public GetAllClassesQueryHandlerTests()
    {
        _classRepositoryMock = Substitute.For<IClassRepository>();
        _memoryCacheMock = Substitute.For<IMemoryCache>();
        _handler = new GetAllClassesQueryHandler(_classRepositoryMock, _memoryCacheMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnListFromRepo_WhenCacheMiss()
    {
        var query = new GetAllClassesQuery();
        var classes = new List<Class> { Class.Create("C1").Value };
        
        _memoryCacheMock.TryGetValue(Arg.Any<object>(), out Arg.Any<object?>()).Returns(false);
        var cacheEntryMock = Substitute.For<ICacheEntry>();
        _memoryCacheMock.CreateEntry(Arg.Any<object>()).Returns(cacheEntryMock);

        _classRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(classes);

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        await _classRepositoryMock.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnListFromCache_WhenCacheHit()
    {
        // Arrange
        var query = new GetAllClassesQuery();
        var cachedClasses = new List<ClassResponse> { new(Guid.NewGuid(), "Cached Class") };
        
        object? outValue = cachedClasses;
        _memoryCacheMock.TryGetValue(Arg.Any<object>(), out Arg.Any<object?>())
            .Returns(x => 
            {
                x[1] = outValue;
                return true;
            });

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(cachedClasses);
        await _classRepositoryMock.DidNotReceive().GetAllAsync(Arg.Any<CancellationToken>());
    }
}