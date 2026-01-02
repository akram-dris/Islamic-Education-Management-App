using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Allocations.Update;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Enums;
using QuranSchool.Domain.Errors;

namespace QuranSchool.UnitTests.Features.Allocations.Update;

public class UpdateAllocationCommandHandlerTests
{
    private readonly IAllocationRepository _allocationRepositoryMock;
    private readonly IUserRepository _userRepositoryMock;
    private readonly IClassRepository _classRepositoryMock;
    private readonly ISubjectRepository _subjectRepositoryMock;
    private readonly UpdateAllocationCommandHandler _handler;

    public UpdateAllocationCommandHandlerTests()
    {
        _allocationRepositoryMock = Substitute.For<IAllocationRepository>();
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _classRepositoryMock = Substitute.For<IClassRepository>();
        _subjectRepositoryMock = Substitute.For<ISubjectRepository>();
        _handler = new UpdateAllocationCommandHandler(
            _allocationRepositoryMock,
            _userRepositoryMock,
            _classRepositoryMock,
            _subjectRepositoryMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAllocationNotFound()
    {
        // Arrange
        var command = new UpdateAllocationCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        _allocationRepositoryMock.GetByIdAsync(command.AllocationId, Arg.Any<CancellationToken>())
            .Returns((Allocation?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Allocation.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenTeacherNotFound()
    {
        // Arrange
        var allocation = Allocation.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()).Value;
        var command = new UpdateAllocationCommand(Guid.NewGuid(), Guid.NewGuid(), allocation.ClassId, allocation.SubjectId);
        
        _allocationRepositoryMock.GetByIdAsync(command.AllocationId, Arg.Any<CancellationToken>())
            .Returns(allocation);
        _userRepositoryMock.GetByIdAsync(command.TeacherId, Arg.Any<CancellationToken>())
            .Returns((User?)null);

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
        var allocation = Allocation.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()).Value;
        var command = new UpdateAllocationCommand(Guid.NewGuid(), Guid.NewGuid(), allocation.ClassId, allocation.SubjectId);
        var user = User.Create("parent", "hash", "Parent", UserRole.Parent).Value;

        _allocationRepositoryMock.GetByIdAsync(command.AllocationId, Arg.Any<CancellationToken>())
            .Returns(allocation);
        _userRepositoryMock.GetByIdAsync(command.TeacherId, Arg.Any<CancellationToken>())
            .Returns(user);

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
        var allocation = Allocation.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()).Value;
        var command = new UpdateAllocationCommand(Guid.NewGuid(), allocation.TeacherId, Guid.NewGuid(), allocation.SubjectId);
        
        _allocationRepositoryMock.GetByIdAsync(command.AllocationId, Arg.Any<CancellationToken>())
            .Returns(allocation);
            
        _classRepositoryMock.GetByIdAsync(command.ClassId, Arg.Any<CancellationToken>())
            .Returns((Class?)null);

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
        var allocation = Allocation.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()).Value;
        var command = new UpdateAllocationCommand(Guid.NewGuid(), allocation.TeacherId, allocation.ClassId, Guid.NewGuid());
        
        _allocationRepositoryMock.GetByIdAsync(command.AllocationId, Arg.Any<CancellationToken>())
            .Returns(allocation);
        _subjectRepositoryMock.GetByIdAsync(command.SubjectId, Arg.Any<CancellationToken>())
            .Returns((Subject?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Subject.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenDuplicateAllocationExists()
    {
        // Arrange
        var allocation = Allocation.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()).Value;
        var newTeacherId = Guid.NewGuid();
        var command = new UpdateAllocationCommand(Guid.NewGuid(), newTeacherId, allocation.ClassId, allocation.SubjectId);
        
        var newTeacher = User.Create("t", "h", "T", UserRole.Teacher).Value;

        _allocationRepositoryMock.GetByIdAsync(command.AllocationId, Arg.Any<CancellationToken>())
            .Returns(allocation);
        _userRepositoryMock.GetByIdAsync(command.TeacherId, Arg.Any<CancellationToken>())
            .Returns(newTeacher);
        
        _allocationRepositoryMock.ExistsAsync(command.TeacherId, command.ClassId, command.SubjectId, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Allocation.Duplicate);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenUpdateIsValid()
    {
        // Arrange
        var allocation = Allocation.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()).Value;
        var newTeacherId = Guid.NewGuid();
        var command = new UpdateAllocationCommand(Guid.NewGuid(), newTeacherId, allocation.ClassId, allocation.SubjectId);
        
        var newTeacher = User.Create("t", "h", "T", UserRole.Teacher).Value;

        _allocationRepositoryMock.GetByIdAsync(command.AllocationId, Arg.Any<CancellationToken>())
            .Returns(allocation);
        _userRepositoryMock.GetByIdAsync(command.TeacherId, Arg.Any<CancellationToken>())
            .Returns(newTeacher);
        
        _allocationRepositoryMock.ExistsAsync(command.TeacherId, command.ClassId, command.SubjectId, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _allocationRepositoryMock.Received(1).UpdateAsync(Arg.Is<Allocation>(a => a.TeacherId == command.TeacherId), Arg.Any<CancellationToken>());
    }
}