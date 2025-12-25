using Microsoft.EntityFrameworkCore;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Enums;

namespace QuranSchool.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<ParentStudent> ParentStudents { get; set; }
    public DbSet<Class> Classes { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Allocation> Allocations { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<Submission> Submissions { get; set; }
    public DbSet<AttendanceSession> AttendanceSessions { get; set; }
    public DbSet<AttendanceRecord> AttendanceRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Enums mapping
        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>();

        modelBuilder.Entity<AttendanceRecord>()
            .Property(ar => ar.Status)
            .HasConversion<string>();

        // ParentStudent
        modelBuilder.Entity<ParentStudent>()
            .HasKey(ps => new { ps.ParentId, ps.StudentId });

        modelBuilder.Entity<ParentStudent>()
            .HasOne(ps => ps.Parent)
            .WithMany()
            .HasForeignKey(ps => ps.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ParentStudent>()
            .HasOne(ps => ps.Student)
            .WithMany()
            .HasForeignKey(ps => ps.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Allocation
        modelBuilder.Entity<Allocation>()
            .HasIndex(a => new { a.TeacherId, a.ClassId, a.SubjectId })
            .IsUnique();
        
        // Enrollment
        modelBuilder.Entity<Enrollment>()
            .HasIndex(e => new { e.StudentId, e.ClassId })
            .IsUnique();

        // Assignment
        modelBuilder.Entity<Submission>()
            .HasIndex(s => new { s.AssignmentId, s.StudentId })
            .IsUnique();

        // Attendance
        modelBuilder.Entity<AttendanceSession>()
            .HasIndex(asess => new { asess.AllocationId, asess.SessionDate })
            .IsUnique();

        modelBuilder.Entity<AttendanceRecord>()
            .HasIndex(ar => new { ar.AttendanceSessionId, ar.StudentId })
            .IsUnique();
    }
}
