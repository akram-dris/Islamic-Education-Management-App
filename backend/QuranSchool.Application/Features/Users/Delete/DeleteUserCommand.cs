using MediatR;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Users.Delete;

public record DeleteUserCommand(Guid UserId) : IRequest<Result>;
