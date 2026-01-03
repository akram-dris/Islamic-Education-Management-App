using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuranSchool.Application.Abstractions.Authentication;
using QuranSchool.Domain.Entities;
using QuranSchool.Infrastructure.Persistence;
using QuranSchool.IntegrationTests.Abstractions;
using Xunit;

namespace QuranSchool.IntegrationTests.Persistence;

public class DbContextAuditingTests : FunctionalTest
{
    [Fact]
    public async Task SaveChangesAsync_ShouldSetAuditProperties_WhenEntityIsAdded()
    {
        // Arrange
        using var scope = ServiceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var user = User.Create("audit_test", "hash", "Audit Test", Domain.Enums.UserRole.Student).Value;

        // Act
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Assert
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldSetSoftDelete_WhenEntityIsDeleted()
    {
        // Arrange
        using var scope = ServiceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var user = User.Create("soft_delete", "hash", "Delete Test", Domain.Enums.UserRole.Student).Value;
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        context.Users.Remove(user);
        await context.SaveChangesAsync();

        // Assert
        var deletedUser = await context.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == user.Id);

        deletedUser.Should().NotBeNull();
        deletedUser!.IsDeleted.Should().BeTrue();
        
        var visibleUser = await context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        visibleUser.Should().BeNull();
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldSetLastModified_WhenEntityIsModified()
    {
        // Arrange
        using var scope = ServiceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var user = User.Create("modify_test", "hash", "Modify Test", Domain.Enums.UserRole.Student).Value;
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        user.Update("New Name", null);
        await context.SaveChangesAsync();

        // Assert
        user.LastModifiedAt.Should().NotBeNull();
        user.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}
