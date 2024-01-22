using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class IdentityConfiguration : IEntityTypeConfiguration<Identity>
{
    public void Configure(EntityTypeBuilder<Identity> builder)
    {
        builder.Property(x => x.ApiKey).IsRequired();
        builder.Property(x => x.ApiSecret).HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.HasIndex(x => x.ApiKey).IsUnique();
    }
}
