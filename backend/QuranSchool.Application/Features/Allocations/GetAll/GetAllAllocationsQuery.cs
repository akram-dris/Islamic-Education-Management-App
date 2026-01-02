using MediatR;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Allocations.GetAll;

public record GetAllAllocationsQuery() : IRequest<Result<List<AllocationResponse>>>;

public record AllocationResponse(
    Guid Id,
    Guid TeacherId,
    string TeacherName,
    Guid ClassId,
    string ClassName,
    Guid SubjectId,
    string SubjectName);
