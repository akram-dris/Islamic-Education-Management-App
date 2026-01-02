using MediatR;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Users.Update;

public record UpdateUserCommand(
    Guid UserId,
    string FullName,
    string? Password) : IRequest<Result>;
