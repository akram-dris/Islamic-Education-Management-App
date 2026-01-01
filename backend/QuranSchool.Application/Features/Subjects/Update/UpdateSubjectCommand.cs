using MediatR;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Subjects.Update;

public record UpdateSubjectCommand(Guid SubjectId, string Name) : IRequest<Result>;
