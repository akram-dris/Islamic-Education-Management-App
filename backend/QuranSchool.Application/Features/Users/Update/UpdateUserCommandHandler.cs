using MediatR;
using QuranSchool.Application.Abstractions.Authentication;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Users.Update;

public sealed class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UpdateUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(Error.NotFound("User.NotFound", "User not found."));
        }

        user.FullName = request.FullName;
        
        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.PasswordHash = _passwordHasher.Hash(request.Password);
        }

        await _userRepository.UpdateAsync(user, cancellationToken);

        return Result.Success();
    }
}
