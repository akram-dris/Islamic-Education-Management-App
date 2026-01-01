using MediatR;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Classes.Update;

public record UpdateClassCommand(Guid ClassId, string Name) : IRequest<Result>;
