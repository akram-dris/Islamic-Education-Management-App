using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Allocations.Create;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Enums;
using QuranSchool.Domain.Errors;

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
    public async Task Handle_ShouldReturnFailure_WhenTeacherNotFound()
    {
        // Arrange
        var command = new CreateAllocationCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        _userRepositoryMock.GetByIdAsync(command.TeacherId).Returns((User?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.TeacherNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserIsNotTeacher()
    {
        // Arrange
        var command = new CreateAllocationCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var user = User.Create("parent", "hash", "Parent", UserRole.Parent).Value;
        _userRepositoryMock.GetByIdAsync(command.TeacherId).Returns(user);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.TeacherNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenClassNotFound()
    {
        // Arrange
        var command = new CreateAllocationCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var teacher = User.Create("teacher", "hash", "Teacher", UserRole.Teacher).Value;
        _userRepositoryMock.GetByIdAsync(command.TeacherId).Returns(teacher);
        _classRepositoryMock.GetByIdAsync(command.ClassId).Returns((Class?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Class.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSubjectNotFound()
    {
        // Arrange
        var command = new CreateAllocationCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var teacher = User.Create("teacher", "hash", "Teacher", UserRole.Teacher).Value;
        var @class = Class.Create("Class").Value;
        _userRepositoryMock.GetByIdAsync(command.TeacherId).Returns(teacher);
        _classRepositoryMock.GetByIdAsync(command.ClassId).Returns(@class);
        _subjectRepositoryMock.GetByIdAsync(command.SubjectId).Returns((Subject?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Subject.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAllocationAlreadyExists()
    {
        // Arrange
        var command = new CreateAllocationCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var teacher = User.Create("teacher", "hash", "Teacher", UserRole.Teacher).Value;
        var @class = Class.Create("Class").Value;
        var subject = Subject.Create("Subject").Value;
        
        _userRepositoryMock.GetByIdAsync(command.TeacherId).Returns(teacher);
        _classRepositoryMock.GetByIdAsync(command.ClassId).Returns(@class);
        _subjectRepositoryMock.GetByIdAsync(command.SubjectId).Returns(subject);
        
        _allocationRepositoryMock.ExistsAsync(command.TeacherId, command.ClassId, command.SubjectId)
            .Returns(true);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Allocation.Duplicate);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenAllocationIsValid()
    {
        // Arrange
        var command = new CreateAllocationCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var teacher = User.Create("teacher", "hash", "Teacher", UserRole.Teacher).Value;
        var @class = Class.Create("Class").Value;
        var subject = Subject.Create("Subject").Value;
        
        _userRepositoryMock.GetByIdAsync(command.TeacherId).Returns(teacher);
        _classRepositoryMock.GetByIdAsync(command.ClassId).Returns(@class);
        _subjectRepositoryMock.GetByIdAsync(command.SubjectId).Returns(subject);
        
        _allocationRepositoryMock.ExistsAsync(command.TeacherId, command.ClassId, command.SubjectId)
            .Returns(false);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _allocationRepositoryMock.Received(1).AddAsync(Arg.Any<Allocation>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenDomainCreationFails()
    {
        // Arrange
        var command = new CreateAllocationCommand(Guid.Empty, Guid.NewGuid(), Guid.NewGuid());
        var teacher = User.Create("teacher", "hash", "Teacher", UserRole.Teacher).Value;
        var @class = Class.Create("Class").Value;
        var subject = Subject.Create("Subject").Value;
        
        _userRepositoryMock.GetByIdAsync(command.TeacherId).Returns(teacher);
        _classRepositoryMock.GetByIdAsync(command.ClassId).Returns(@class);
        _subjectRepositoryMock.GetByIdAsync(command.SubjectId).Returns(subject);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Allocation.EmptyTeacherId);
    }
}