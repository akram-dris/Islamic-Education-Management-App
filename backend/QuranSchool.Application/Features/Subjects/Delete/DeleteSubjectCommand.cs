using MediatR;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Subjects.Delete;

public record DeleteSubjectCommand(Guid SubjectId) : IRequest<Result>;
