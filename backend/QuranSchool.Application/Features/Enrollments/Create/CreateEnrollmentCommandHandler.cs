using MediatR;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Entities;

namespace QuranSchool.Application.Features.Enrollments.Create;

internal sealed class CreateEnrollmentCommandHandler : IRequestHandler<CreateEnrollmentCommand, Result<Guid>>
{
    private readonly IEnrollmentRepository _enrollmentRepository;

    public CreateEnrollmentCommandHandler(IEnrollmentRepository enrollmentRepository)
    {
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<Result<Guid>> Handle(CreateEnrollmentCommand request, CancellationToken cancellationToken)
    {
        if (await _enrollmentRepository.ExistsAsync(request.StudentId, request.ClassId, cancellationToken))
        {
            return Result<Guid>.Failure(Error.Conflict("Enrollment.Duplicate", "Student is already enrolled in this class."));
        }

        var enrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            StudentId = request.StudentId,
            ClassId = request.ClassId
        };

        await _enrollmentRepository.AddAsync(enrollment, cancellationToken);

        return enrollment.Id;
    }
}
