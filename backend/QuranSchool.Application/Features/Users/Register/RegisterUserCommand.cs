using MediatR;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Enums;

namespace QuranSchool.Application.Features.Users.Register;

public record RegisterUserCommand(
    string Username,
    string Password,
    string FullName,
    UserRole Role) : IRequest<Result<Guid>>;
