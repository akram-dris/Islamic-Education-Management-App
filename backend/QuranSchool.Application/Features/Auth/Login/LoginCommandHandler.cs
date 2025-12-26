using MediatR;
using QuranSchool.Application.Abstractions.Authentication;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Auth.Login;

internal sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);

        if (user is null)
        {
            return Result<LoginResponse>.Failure(Error.Unauthorized("Auth.InvalidCredentials", "Invalid username or password."));
        }

        bool verified = _passwordHasher.Verify(request.Password, user.PasswordHash);

        if (!verified)
        {
            return Result<LoginResponse>.Failure(Error.Unauthorized("Auth.InvalidCredentials", "Invalid username or password."));
        }

        string token = _jwtProvider.Generate(user);

        return new LoginResponse(
            token,
            user.Username,
            user.FullName,
            user.Role.ToString());
    }
}
