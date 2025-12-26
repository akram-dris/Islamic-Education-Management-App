using MediatR;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Submissions.Create;

public record CreateSubmissionCommand(
    Guid AssignmentId,
    Guid StudentId,
    string FileUrl) : IRequest<Result<Guid>>;
