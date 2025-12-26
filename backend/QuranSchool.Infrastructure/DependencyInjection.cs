using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuranSchool.Application.Abstractions.Authentication;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Infrastructure.Authentication;
using QuranSchool.Infrastructure.Persistence;
using QuranSchool.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace QuranSchool.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration["DEFAULT_CONNECTION"]));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IClassRepository, ClassRepository>();
        services.AddScoped<ISubjectRepository, SubjectRepository>();
        services.AddScoped<IAllocationRepository, AllocationRepository>();
        services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtProvider, JwtProvider>();

        services.Configure<JwtOptions>(options => 
        {
            options.Issuer = configuration["JWT_ISSUER"] ?? string.Empty;
            options.Audience = configuration["JWT_AUDIENCE"] ?? string.Empty;
            options.SecretKey = configuration["JWT_SECRET_KEY"] ?? string.Empty;
        });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["JWT_ISSUER"],
                ValidAudience = configuration["JWT_AUDIENCE"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["JWT_SECRET_KEY"] ?? string.Empty))
            });

        services.AddAuthorization();

        return services;
    }
}
