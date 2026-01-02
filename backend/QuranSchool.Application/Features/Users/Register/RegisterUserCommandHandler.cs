using MediatR;
using QuranSchool.Application.Abstractions.Authentication;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Errors;

namespace QuranSchool.Application.Features.Users.Register;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);
        if (existingUser is not null)
        {
            return Result<Guid>.Failure(DomainErrors.User.DuplicateUsername);
        }

        var userResult = User.Create(
            request.Username,
            _passwordHasher.Hash(request.Password),
            request.FullName,
            request.Role);

        if (userResult.IsFailure)
        {
            return Result<Guid>.Failure(userResult.Error);
        }

        var user = userResult.Value;

        await _userRepository.AddAsync(user, cancellationToken);

        return user.Id;
    }
}
