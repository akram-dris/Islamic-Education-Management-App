using MediatR;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Errors;

namespace QuranSchool.Application.Features.Enrollments.Delete;

public sealed class DeleteEnrollmentCommandHandler : IRequestHandler<DeleteEnrollmentCommand, Result>
{
    private readonly IEnrollmentRepository _enrollmentRepository;

    public DeleteEnrollmentCommandHandler(IEnrollmentRepository enrollmentRepository)
    {
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<Result> Handle(DeleteEnrollmentCommand request, CancellationToken cancellationToken)
    {
        var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId, cancellationToken);
        if (enrollment is null)
        {
            return Result.Failure(DomainErrors.Enrollment.NotFound);
        }

        await _enrollmentRepository.DeleteAsync(enrollment, cancellationToken);

        return Result.Success();
    }
}
