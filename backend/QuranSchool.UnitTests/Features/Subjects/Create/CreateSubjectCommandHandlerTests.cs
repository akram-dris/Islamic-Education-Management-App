using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Subjects.Create;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Errors;
using Microsoft.Extensions.Caching.Memory;

namespace QuranSchool.UnitTests.Features.Subjects.Create;

public class CreateSubjectCommandHandlerTests
{
    private readonly ISubjectRepository _subjectRepositoryMock;
    private readonly IMemoryCache _memoryCacheMock;
    private readonly CreateSubjectCommandHandler _handler;

    public CreateSubjectCommandHandlerTests()
    {
        _subjectRepositoryMock = Substitute.For<ISubjectRepository>();
        _memoryCacheMock = Substitute.For<IMemoryCache>();
        _handler = new CreateSubjectCommandHandler(_subjectRepositoryMock, _memoryCacheMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenNameIsDuplicate()
    {
        var command = new CreateSubjectCommand("Duplicate");
        _subjectRepositoryMock.ExistsAsync(command.Name).Returns(true);

        var result = await _handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Subject.DuplicateName);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSubjectCreateFails()
    {
        var command = new CreateSubjectCommand(""); // Empty name
        _subjectRepositoryMock.ExistsAsync(command.Name).Returns(false);

        var result = await _handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Subject.EmptyName);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenDataIsValid()
    {
        var command = new CreateSubjectCommand("New Subject");
        _subjectRepositoryMock.ExistsAsync(command.Name).Returns(false);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        await _subjectRepositoryMock.Received(1).AddAsync(Arg.Any<Subject>(), Arg.Any<CancellationToken>());
    }
}