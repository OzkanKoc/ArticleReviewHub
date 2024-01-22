using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.ToTable("Article");
        builder.Property(x => x.Author).IsRequired().HasMaxLength(300);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(300);
        builder.Property(x => x.ArticleContent).IsRequired().HasColumnType("Text");
    }
}
