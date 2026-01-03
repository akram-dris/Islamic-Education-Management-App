using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace QuranSchool.IntegrationTests.Abstractions;

public static class JwtTestHelper
{
    private const string SecretKey = "super-secret-key-that-is-at-least-32-characters-long";
    private const string Issuer = "QuranSchool";
    private const string Audience = "QuranSchool";

    public static string GenerateToken(IEnumerable<Claim> claims)
    {
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            Issuer,
            Audience,
            claims,
            null,
            DateTime.UtcNow.AddHours(1),
            signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
