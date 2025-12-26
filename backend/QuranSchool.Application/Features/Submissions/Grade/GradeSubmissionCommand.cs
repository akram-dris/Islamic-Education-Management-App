using MediatR;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Submissions.Grade;

public record GradeSubmissionCommand(
    Guid SubmissionId,
    decimal Grade,
    string? Remarks) : IRequest<Result>;
