using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using QuranSchool.Application.Abstractions.Authentication;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Entities;

namespace QuranSchool.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    private readonly IUserContext? _userContext;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IUserContext? userContext = null) : base(options)
    {
        _userContext = userContext;
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Class> Classes { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Allocation> Allocations { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<ParentStudent> ParentStudents { get; set; }
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<Submission> Submissions { get; set; }
    public DbSet<AttendanceSession> AttendanceSessions { get; set; }
    public DbSet<AttendanceRecord> AttendanceRecords { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries<Entity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted);

        foreach (var entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Entity.CreatedAt = DateTime.UtcNow;
                entityEntry.Entity.CreatedBy = _userContext?.UserId;
            }
            else if (entityEntry.State == EntityState.Modified)
            {
                entityEntry.Entity.LastModifiedAt = DateTime.UtcNow;
                entityEntry.Entity.LastModifiedBy = _userContext?.UserId;
            }
            else if (entityEntry.State == EntityState.Deleted)
            {
                entityEntry.State = EntityState.Modified;
                entityEntry.Entity.IsDeleted = true;
                entityEntry.Entity.LastModifiedAt = DateTime.UtcNow;
                entityEntry.Entity.LastModifiedBy = _userContext?.UserId;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(Entity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(ConvertFilterExpression<Entity>(e => !e.IsDeleted, entityType.ClrType));
            }
        }

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    private static System.Linq.Expressions.LambdaExpression ConvertFilterExpression<TInterface>(
        System.Linq.Expressions.Expression<Func<TInterface, bool>> filterExpression,
        Type entityType)
    {
        var newParam = System.Linq.Expressions.Expression.Parameter(entityType);
        var newBody = ReplacingExpressionVisitor.Replace(filterExpression.Parameters.Single(), newParam, filterExpression.Body);
        return System.Linq.Expressions.Expression.Lambda(newBody, newParam);
    }
}