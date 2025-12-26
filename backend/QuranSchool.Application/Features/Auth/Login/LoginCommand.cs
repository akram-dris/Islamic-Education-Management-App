using MediatR;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Auth.Login;

public record LoginCommand(string Username, string Password) : IRequest<Result<LoginResponse>>;

public record LoginResponse(string Token, string Username, string FullName, string Role);
