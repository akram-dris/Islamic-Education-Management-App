using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Subjects.Create;

namespace QuranSchool.UnitTests.Features.Subjects.Create;

public class CreateSubjectCommandHandlerTests
{
    private readonly ISubjectRepository _subjectRepositoryMock;
    private readonly CreateSubjectCommandHandler _handler;

    public CreateSubjectCommandHandlerTests()
    {
        _subjectRepositoryMock = Substitute.For<ISubjectRepository>();
        _handler = new CreateSubjectCommandHandler(_subjectRepositoryMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSubjectNameIsNotUnique()
    {
        // Arrange
        var command = new CreateSubjectCommand("Quran");
        _subjectRepositoryMock.ExistsAsync(command.Name, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Subject.DuplicateName");
    }
}
