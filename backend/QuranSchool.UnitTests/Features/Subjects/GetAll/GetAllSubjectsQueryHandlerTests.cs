using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Subjects.GetAll;
using QuranSchool.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace QuranSchool.UnitTests.Features.Subjects.GetAll;

public class GetAllSubjectsQueryHandlerTests
{
    private readonly ISubjectRepository _subjectRepositoryMock;
    private readonly IMemoryCache _memoryCacheMock;
    private readonly GetAllSubjectsQueryHandler _handler;

    public GetAllSubjectsQueryHandlerTests()
    {
        _subjectRepositoryMock = Substitute.For<ISubjectRepository>();
        _memoryCacheMock = Substitute.For<IMemoryCache>();
        _handler = new GetAllSubjectsQueryHandler(_subjectRepositoryMock, _memoryCacheMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnListFromRepo_WhenCacheMiss()
    {
        var query = new GetAllSubjectsQuery();
        var subjects = new List<Subject> { Subject.Create("S1").Value };
        
        _memoryCacheMock.TryGetValue(Arg.Any<object>(), out Arg.Any<object?>()).Returns(false);
        var cacheEntryMock = Substitute.For<ICacheEntry>();
        _memoryCacheMock.CreateEntry(Arg.Any<object>()).Returns(cacheEntryMock);

        _subjectRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(subjects);

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_ShouldReturnListFromCache_WhenCacheHit()
    {
        // Arrange
        var query = new GetAllSubjectsQuery();
        var cachedSubjects = new List<SubjectResponse> { new(Guid.NewGuid(), "Cached Subject") };
        
        object? outValue = cachedSubjects;
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
        result.Value.Should().BeEquivalentTo(cachedSubjects);
        await _subjectRepositoryMock.DidNotReceive().GetAllAsync(Arg.Any<CancellationToken>());
    }
}
