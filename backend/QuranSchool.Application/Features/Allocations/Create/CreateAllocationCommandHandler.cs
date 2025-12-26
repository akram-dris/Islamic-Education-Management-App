using MediatR;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Enums;

namespace QuranSchool.Application.Features.Allocations.Create;

internal sealed class CreateAllocationCommandHandler : IRequestHandler<CreateAllocationCommand, Result<Guid>>
{
    private readonly IAllocationRepository _allocationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IClassRepository _classRepository;
    private readonly ISubjectRepository _subjectRepository;

    public CreateAllocationCommandHandler(
        IAllocationRepository allocationRepository,
        IUserRepository userRepository,
        IClassRepository classRepository,
        ISubjectRepository subjectRepository)
    {
        _allocationRepository = allocationRepository;
        _userRepository = userRepository;
        _classRepository = classRepository;
        _subjectRepository = subjectRepository;
    }

    public async Task<Result<Guid>> Handle(CreateAllocationCommand request, CancellationToken cancellationToken)
    {
        var teacher = await _userRepository.GetByIdAsync(request.TeacherId, cancellationToken);
        if (teacher is null || teacher.Role != UserRole.Teacher)
        {
            return Result<Guid>.Failure(Error.NotFound("User.TeacherNotFound", "Teacher not found."));
        }

        var @class = await _classRepository.GetByIdAsync(request.ClassId, cancellationToken);
        if (@class is null)
        {
            return Result<Guid>.Failure(Error.NotFound("Class.NotFound", "Class not found."));
        }

        var subject = await _subjectRepository.GetByIdAsync(request.SubjectId, cancellationToken);
        if (subject is null)
        {
            return Result<Guid>.Failure(Error.NotFound("Subject.NotFound", "Subject not found."));
        }

        var allocation = new Allocation
        {
            Id = Guid.NewGuid(),
            TeacherId = request.TeacherId,
            ClassId = request.ClassId,
            SubjectId = request.SubjectId
        };

        await _allocationRepository.AddAsync(allocation, cancellationToken);

        return allocation.Id;
    }
}
