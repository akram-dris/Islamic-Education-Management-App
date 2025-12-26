using MediatR;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Classes.Create;

public record CreateClassCommand(string Name) : IRequest<Result<Guid>>;
