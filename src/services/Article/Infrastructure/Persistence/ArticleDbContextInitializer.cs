using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence;

public class ArticleDbContextInitializer(ILogger<ArticleDbContextInitializer> logger, ArticleDbContext context)
{
    public async Task Initialize()
    {
        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }

    public async Task Seed()
    {
        try
        {
            if (!await context.Set<Identity>().AnyAsync())
            {
                const string apiKey = "article-api-key";
                const string apiSecret = "article-api-secret";

                FormattableString query = $"""
                                           INSERT Identity (ApiKey, ApiSecret)
                                           VALUES ({apiKey}, {apiSecret})
                                           """;

                await context.Database.ExecuteSqlAsync(query);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }
}
