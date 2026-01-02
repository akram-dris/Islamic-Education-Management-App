using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using QuranSchool.Application.Abstractions.Authentication;

namespace QuranSchool.Infrastructure.Authentication;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId => 
        _httpContextAccessor.HttpContext?.User is not null &&
        Guid.TryParse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) 
            ? userId 
            : Guid.Empty;
}
