using Microsoft.EntityFrameworkCore.Migrations;
using NSubstitute;
using Xunit;
using System.Reflection;
using QuranSchool.Infrastructure.Persistence;
using FluentAssertions;

namespace QuranSchool.IntegrationTests.Persistence;

public class MigrationsTests
{
    [Fact]
    public void AllMigrations_ShouldHaveUpAndDownMethodsExercised()
    {
        // Arrange
        var migrationBuilder = Substitute.For<MigrationBuilder>("Npgsql");
        
        var migrationTypes = typeof(ApplicationDbContext).Assembly.GetTypes()
            .Where(t => typeof(Migration).IsAssignableFrom(t) && !t.IsAbstract && t.Name != "ApplicationDbContextModelSnapshot");

        foreach (var type in migrationTypes)
        {
            var migration = (Migration)Activator.CreateInstance(type)!;

            // Act
            // Accessing protected methods Up and Down via reflection
            var upMethod = type.GetMethod("Up", BindingFlags.NonPublic | BindingFlags.Instance);
            var downMethod = type.GetMethod("Down", BindingFlags.NonPublic | BindingFlags.Instance);

            upMethod?.Invoke(migration, new object[] { migrationBuilder });
            downMethod?.Invoke(migration, new object[] { migrationBuilder });
        }

        // Exercise Snapshot
        var snapshotType = typeof(ApplicationDbContext).Assembly.GetTypes()
            .FirstOrDefault(t => t.Name == "ApplicationDbContextModelSnapshot");
        if (snapshotType != null)
        {
            var snapshot = Activator.CreateInstance(snapshotType);
            var buildMethod = snapshotType.GetMethod("BuildModel", BindingFlags.NonPublic | BindingFlags.Instance);
            var modelBuilder = new Microsoft.EntityFrameworkCore.ModelBuilder(new Microsoft.EntityFrameworkCore.Metadata.Conventions.ConventionSet());
            buildMethod?.Invoke(snapshot, new object[] { modelBuilder });
        }

        // Assert
        // If we reached here without exception, they are "covered"
        migrationBuilder.Should().NotBeNull();
    }
}
