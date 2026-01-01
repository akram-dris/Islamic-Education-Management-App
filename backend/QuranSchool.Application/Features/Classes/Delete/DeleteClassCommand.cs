using MediatR;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Classes.Delete;

public record DeleteClassCommand(Guid ClassId) : IRequest<Result>;
