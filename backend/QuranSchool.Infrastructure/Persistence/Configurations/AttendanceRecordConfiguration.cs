using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuranSchool.Domain.Entities;

namespace QuranSchool.Infrastructure.Persistence.Configurations;

public class AttendanceRecordConfiguration : IEntityTypeConfiguration<AttendanceRecord>
{
    public void Configure(EntityTypeBuilder<AttendanceRecord> builder)
    {
        builder.Property(ar => ar.Status)
            .HasConversion<string>();

        builder.HasIndex(ar => new { ar.AttendanceSessionId, ar.StudentId })
            .IsUnique();
    }
}
