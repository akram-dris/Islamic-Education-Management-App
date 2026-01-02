using Microsoft.EntityFrameworkCore;
using QuranSchool.Application.Abstractions.Authentication;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Enums;
using QuranSchool.Infrastructure.Persistence;

namespace QuranSchool.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        await context.Database.MigrateAsync();

        if (!await context.Users.AnyAsync(u => u.Role == UserRole.Admin))
        {
            var admin = User.Create(
                "admin",
                passwordHasher.Hash("admin123"),
                "System Administrator",
                UserRole.Admin).Value;

            await context.Users.AddAsync(admin);
            await context.SaveChangesAsync();
        }
    }
}
