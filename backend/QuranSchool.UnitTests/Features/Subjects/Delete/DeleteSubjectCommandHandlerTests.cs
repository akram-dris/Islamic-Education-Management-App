using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Subjects.Delete;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Errors;
using Microsoft.Extensions.Caching.Memory;

namespace QuranSchool.UnitTests.Features.Subjects.Delete;

public class DeleteSubjectCommandHandlerTests
{
    private readonly ISubjectRepository _subjectRepositoryMock;
    private readonly IMemoryCache _memoryCacheMock;
    private readonly DeleteSubjectCommandHandler _handler;

    public DeleteSubjectCommandHandlerTests()
    {
        _subjectRepositoryMock = Substitute.For<ISubjectRepository>();
        _memoryCacheMock = Substitute.For<IMemoryCache>();
        _handler = new DeleteSubjectCommandHandler(_subjectRepositoryMock, _memoryCacheMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSubjectNotFound()
    {
        var command = new DeleteSubjectCommand(Guid.NewGuid());
        _subjectRepositoryMock.GetByIdAsync(command.SubjectId, Arg.Any<CancellationToken>())
            .Returns((Subject?)null);

        var result = await _handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Subject.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenSubjectExists()
    {
        var command = new DeleteSubjectCommand(Guid.NewGuid());
        var subject = Subject.Create("Name").Value;
        
        _subjectRepositoryMock.GetByIdAsync(command.SubjectId, Arg.Any<CancellationToken>())
            .Returns(subject);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        await _subjectRepositoryMock.Received(1).DeleteAsync(subject, Arg.Any<CancellationToken>());
    }
}
