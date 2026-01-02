using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Subjects.Update;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Errors;
using Microsoft.Extensions.Caching.Memory;

namespace QuranSchool.UnitTests.Features.Subjects.Update;

public class UpdateSubjectCommandHandlerTests
{
    private readonly ISubjectRepository _subjectRepositoryMock;
    private readonly IMemoryCache _memoryCacheMock;
    private readonly UpdateSubjectCommandHandler _handler;

    public UpdateSubjectCommandHandlerTests()
    {
        _subjectRepositoryMock = Substitute.For<ISubjectRepository>();
        _memoryCacheMock = Substitute.For<IMemoryCache>();
        _handler = new UpdateSubjectCommandHandler(
            _subjectRepositoryMock,
            _memoryCacheMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSubjectNotFound()
    {
        // Arrange
        var command = new UpdateSubjectCommand(Guid.NewGuid(), "New Name");
        _subjectRepositoryMock.GetByIdAsync(command.SubjectId, Arg.Any<CancellationToken>())
            .Returns((Subject?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Subject.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenNameIsDuplicate()
    {
        // Arrange
        var command = new UpdateSubjectCommand(Guid.NewGuid(), "Duplicate Name");
        var existingSubject = Subject.Create("Old Name").Value;
        
        _subjectRepositoryMock.GetByIdAsync(command.SubjectId, Arg.Any<CancellationToken>())
            .Returns(existingSubject);
        _subjectRepositoryMock.ExistsAsync(command.Name, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Subject.DuplicateName);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenUpdateIsValid()
    {
        // Arrange
        var command = new UpdateSubjectCommand(Guid.NewGuid(), "New Name");
        var existingSubject = Subject.Create("Old Name").Value;
        
        _subjectRepositoryMock.GetByIdAsync(command.SubjectId, Arg.Any<CancellationToken>())
            .Returns(existingSubject);
        _subjectRepositoryMock.ExistsAsync(command.Name, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _subjectRepositoryMock.Received(1).UpdateAsync(Arg.Is<Subject>(s => s.Name == command.Name), Arg.Any<CancellationToken>());
    }
}
