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
        var user = User.Create("u", "p", "n", UserRole.Student).Value;
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
        var teacher = User.Create("t", "p", "n", UserRole.Teacher).Value;
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
        var teacher = User.Create("t", "p", "n", UserRole.Teacher).Value;
        var cls = Class.Create("c").Value;
        
        _userRepositoryMock.GetByIdAsync(command.TeacherId).Returns(teacher);
        _classRepositoryMock.GetByIdAsync(command.ClassId).Returns(cls);
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
        
        _userRepositoryMock.GetByIdAsync(command.TeacherId).Returns(User.Create("d", "d", "d", UserRole.Teacher).Value);
        _classRepositoryMock.GetByIdAsync(command.ClassId).Returns(Class.Create("dummy").Value);
        _subjectRepositoryMock.GetByIdAsync(command.SubjectId).Returns(Subject.Create("dummy").Value);
        
        _allocationRepositoryMock.ExistsAsync(command.TeacherId, command.ClassId, command.SubjectId)
            .Returns(true);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Allocation.Duplicate);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAllocationCreationFails()
    {
        // Arrange - use Guid.Empty to trigger domain validation failure
        var command = new CreateAllocationCommand(Guid.Empty, Guid.NewGuid(), Guid.NewGuid());
        
        var teacher = User.Create("t", "p", "n", UserRole.Teacher).Value;
        var cls = Class.Create("c").Value;
        var subject = Subject.Create("s").Value;

        _userRepositoryMock.GetByIdAsync(command.TeacherId).Returns(teacher); // Mock will ignore the Guid mismatch for simplicity or we can match Arg.Any
        _classRepositoryMock.GetByIdAsync(command.ClassId).Returns(cls);
        _subjectRepositoryMock.GetByIdAsync(command.SubjectId).Returns(subject);

        // Need to ensure the mocks return for the empty guid if that's what we pass, 
        // but typically the validation happens inside Allocation.Create. 
        // Logic: The handler calls GetByIdAsync BEFORE Allocation.Create.
        // If we pass Guid.Empty, GetByIdAsync will be called with Guid.Empty.
        // We need to make sure our mocks respond to that or we skip the early checks.
        // Actually, for this specific test, we want to fail the 'teacher check' if it's null, 
        // but here we want to test 'Allocation.Create' failure.
        // Allocation.Create fails if TeacherId is empty. 
        // But the Handler checks for Teacher existence first.
        // If we pass Guid.Empty, the repo probably returns null? 
        // Let's assume the repo returns a valid teacher even for Guid.Empty to test the Create logic.
        
        _userRepositoryMock.GetByIdAsync(Guid.Empty).Returns(teacher);
        
        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Allocation.EmptyTeacherId);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenAllocationIsValid()
    {
        // Arrange
        var command = new CreateAllocationCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var teacher = User.Create("t", "p", "n", UserRole.Teacher).Value;
        var cls = Class.Create("c").Value;
        var subject = Subject.Create("s").Value;

        _userRepositoryMock.GetByIdAsync(command.TeacherId).Returns(teacher);
        _classRepositoryMock.GetByIdAsync(command.ClassId).Returns(cls);
        _subjectRepositoryMock.GetByIdAsync(command.SubjectId).Returns(subject);
        _allocationRepositoryMock.ExistsAsync(command.TeacherId, command.ClassId, command.SubjectId).Returns(false);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        await _allocationRepositoryMock.Received(1).AddAsync(Arg.Any<Allocation>(), Arg.Any<CancellationToken>());
    }
}
