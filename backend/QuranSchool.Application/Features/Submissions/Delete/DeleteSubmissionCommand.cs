using MediatR;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Submissions.Delete;

public record DeleteSubmissionCommand(Guid SubmissionId) : IRequest<Result>;
