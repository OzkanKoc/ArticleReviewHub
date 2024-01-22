using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.ToTable("Article");
        builder.Property(x => x.Author).HasMaxLength(300);
        builder.Property(x => x.Title).HasMaxLength(300);
        builder.Property(x => x.ArticleContent).HasColumnType("Text");
        builder.Property(x => x.PublishDate).HasDefaultValue(DateTime.Now);
    }
}
