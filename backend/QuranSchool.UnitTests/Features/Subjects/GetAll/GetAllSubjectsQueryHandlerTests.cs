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
}
