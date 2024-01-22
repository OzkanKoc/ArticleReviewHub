using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ArticleDbContext(DbContextOptions<ArticleDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
