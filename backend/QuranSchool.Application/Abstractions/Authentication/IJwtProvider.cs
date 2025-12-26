using QuranSchool.Domain.Entities;

namespace QuranSchool.Application.Abstractions.Authentication;

public interface IJwtProvider
{
    string Generate(User user);
}
