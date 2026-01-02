using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Enums;
using QuranSchool.Domain.Errors;

namespace QuranSchool.Domain.Entities;

public class User : Entity
{
    private User(string username, string passwordHash, string fullName, UserRole role)
    {
        Username = username;
        PasswordHash = passwordHash;
        FullName = fullName;
        Role = role;
    }

    private User()
    {
    }

    public string Username { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }

    public static Result<User> Create(string username, string passwordHash, string fullName, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return Result<User>.Failure(DomainErrors.User.EmptyUsername);
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            return Result<User>.Failure(DomainErrors.User.EmptyPassword);
        }

        if (string.IsNullOrWhiteSpace(fullName))
        {
            return Result<User>.Failure(DomainErrors.User.EmptyFullName);
        }

        return new User(username, passwordHash, fullName, role);
    }

    public Result Update(string fullName, string? passwordHash)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return Result.Failure(DomainErrors.User.EmptyFullName);
        }

        FullName = fullName;

        if (!string.IsNullOrWhiteSpace(passwordHash))
        {
            PasswordHash = passwordHash;
        }

        return Result.Success();
    }
}