using MediatR;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Errors;

namespace QuranSchool.Application.Features.Enrollments.Create;

public sealed class CreateEnrollmentCommandHandler : IRequestHandler<CreateEnrollmentCommand, Result<Guid>>
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
            return Result<Guid>.Failure(DomainErrors.Enrollment.Duplicate);
        }

        var enrollment = Enrollment.Create(request.StudentId, request.ClassId).Value;

        await _enrollmentRepository.AddAsync(enrollment, cancellationToken);

        return enrollment.Id;
    }
}
