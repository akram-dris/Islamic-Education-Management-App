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
    private readonly IMemoryCache _cacheMock;
    private readonly GetAllSubjectsQueryHandler _handler;

    public GetAllSubjectsQueryHandlerTests()
    {
        _subjectRepositoryMock = Substitute.For<ISubjectRepository>();
        _cacheMock = Substitute.For<IMemoryCache>();
        _handler = new GetAllSubjectsQueryHandler(_subjectRepositoryMock, _cacheMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllSubjects()
    {
        // Arrange
        var subject = Subject.Create("Subject A").Value;
        _subjectRepositoryMock.GetAllAsync(default).Returns(new List<Subject> { subject });

        // Act
        var result = await _handler.Handle(new GetAllSubjectsQuery(), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].Name.Should().Be("Subject A");
    }
}
