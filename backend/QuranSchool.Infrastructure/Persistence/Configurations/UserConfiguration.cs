using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuranSchool.Domain.Entities;

namespace QuranSchool.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasQueryFilter(u => !u.IsDeleted);

        builder.Property(u => u.Role)
            .HasConversion<string>();
    }
}
