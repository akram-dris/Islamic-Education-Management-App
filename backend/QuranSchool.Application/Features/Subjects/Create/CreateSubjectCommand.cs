using MediatR;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Subjects.Create;

public record CreateSubjectCommand(string Name) : IRequest<Result<Guid>>;
