using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Allocations.Create;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Enums;

namespace QuranSchool.UnitTests.Features.Allocations.Create;

public class CreateAllocationCommandHandlerTests
{
    private readonly IAllocationRepository _allocationRepositoryMock;
    private readonly IUserRepository _userRepositoryMock;
    private readonly IClassRepository _classRepositoryMock;
    private readonly ISubjectRepository _subjectRepositoryMock;
    private readonly CreateAllocationCommandHandler _handler;

    public CreateAllocationCommandHandlerTests()
    {
        _allocationRepositoryMock = Substitute.For<IAllocationRepository>();
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _classRepositoryMock = Substitute.For<IClassRepository>();
        _subjectRepositoryMock = Substitute.For<ISubjectRepository>();
        _handler = new CreateAllocationCommandHandler(
            _allocationRepositoryMock,
            _userRepositoryMock,
            _classRepositoryMock,
            _subjectRepositoryMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAllocationAlreadyExists()
    {
        // Arrange
        var command = new CreateAllocationCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        
        _userRepositoryMock.GetByIdAsync(command.TeacherId).Returns(new User { Role = UserRole.Teacher });
        _classRepositoryMock.GetByIdAsync(command.ClassId).Returns(new Class());
        _subjectRepositoryMock.GetByIdAsync(command.SubjectId).Returns(new Subject());
        
        _allocationRepositoryMock.ExistsAsync(command.TeacherId, command.ClassId, command.SubjectId)
            .Returns(true);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Allocation.Duplicate");
    }
}
