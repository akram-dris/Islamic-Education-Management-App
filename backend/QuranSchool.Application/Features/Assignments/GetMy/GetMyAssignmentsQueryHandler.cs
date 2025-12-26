using MediatR;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Assignments.GetMy;

public sealed class GetMyAssignmentsQueryHandler : IRequestHandler<GetMyAssignmentsQuery, Result<List<AssignmentResponse>>>
{
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;

    public GetMyAssignmentsQueryHandler(
        IAssignmentRepository assignmentRepository,
        IEnrollmentRepository enrollmentRepository)
    {
        _assignmentRepository = assignmentRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<Result<List<AssignmentResponse>>> Handle(GetMyAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var enrollments = await _enrollmentRepository.GetByStudentIdAsync(request.StudentId, cancellationToken);
        var assignments = new List<AssignmentResponse>();

        foreach (var enrollment in enrollments)
        {
            var classAssignments = await _assignmentRepository.GetByClassIdAsync(enrollment.ClassId, cancellationToken);
            assignments.AddRange(classAssignments.Select(a => new AssignmentResponse(
                a.Id,
                a.AllocationId,
                a.Title,
                a.Description,
                a.DueDate,
                a.CreatedAt)));
        }

        return assignments.OrderByDescending(a => a.CreatedAt).ToList();
    }
}