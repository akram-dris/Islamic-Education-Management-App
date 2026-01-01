using MediatR;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Enums;
using QuranSchool.Domain.Errors;

namespace QuranSchool.Application.Features.Allocations.Update;

public sealed class UpdateAllocationCommandHandler : IRequestHandler<UpdateAllocationCommand, Result>
{
    private readonly IAllocationRepository _allocationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IClassRepository _classRepository;
    private readonly ISubjectRepository _subjectRepository;

    public UpdateAllocationCommandHandler(
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

    public async Task<Result> Handle(UpdateAllocationCommand request, CancellationToken cancellationToken)
    {
        var allocation = await _allocationRepository.GetByIdAsync(request.AllocationId, cancellationToken);
        if (allocation is null)
        {
            return Result.Failure(DomainErrors.Allocation.NotFound);
        }

        // Validate Teacher
        if (allocation.TeacherId != request.TeacherId)
        {
            var teacher = await _userRepository.GetByIdAsync(request.TeacherId, cancellationToken);
            if (teacher is null || teacher.Role != UserRole.Teacher)
            {
                return Result.Failure(DomainErrors.User.TeacherNotFound);
            }
        }

        // Validate Class
        if (allocation.ClassId != request.ClassId)
        {
            var @class = await _classRepository.GetByIdAsync(request.ClassId, cancellationToken);
            if (@class is null)
            {
                return Result.Failure(DomainErrors.Class.NotFound);
            }
        }

        // Validate Subject
        if (allocation.SubjectId != request.SubjectId)
        {
            var subject = await _subjectRepository.GetByIdAsync(request.SubjectId, cancellationToken);
            if (subject is null)
            {
                return Result.Failure(DomainErrors.Subject.NotFound);
            }
        }

        // Check for duplicate if any key changed
        if (allocation.TeacherId != request.TeacherId || 
            allocation.ClassId != request.ClassId || 
            allocation.SubjectId != request.SubjectId)
        {
            if (await _allocationRepository.ExistsAsync(request.TeacherId, request.ClassId, request.SubjectId, cancellationToken))
            {
                return Result.Failure(DomainErrors.Allocation.Duplicate);
            }
        }

        allocation.TeacherId = request.TeacherId;
        allocation.ClassId = request.ClassId;
        allocation.SubjectId = request.SubjectId;

        await _allocationRepository.UpdateAsync(allocation, cancellationToken);

        return Result.Success();
    }
}
