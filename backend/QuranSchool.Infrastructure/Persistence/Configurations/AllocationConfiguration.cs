using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuranSchool.Domain.Entities;

namespace QuranSchool.Infrastructure.Persistence.Configurations;

public class AllocationConfiguration : IEntityTypeConfiguration<Allocation>
{
    public void Configure(EntityTypeBuilder<Allocation> builder)
    {
        builder.HasIndex(a => new { a.TeacherId, a.ClassId, a.SubjectId })
            .IsUnique();
    }
}
