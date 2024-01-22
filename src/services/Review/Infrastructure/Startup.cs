using Application.Common.ApiClient.Article;
using Application.Common.Repositories;
using EntityFramework.Exceptions.MySQL.Pomelo;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application.Common.Caching;
using Application.Common.Serializer;
using Infrastructure.ApiClient.Article;
using Infrastructure.Caching;
using Infrastructure.Serializer;

namespace Infrastructure;

public static class Startup
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        var mySqlConnectionString = configuration.GetConnectionString("DatabaseConnection");

        services.AddSingleton(_ => ServerVersion.AutoDetect(mySqlConnectionString));

        services.AddDbContext<ReviewDbContext>((sp, dbContextOptions) =>
        {
            dbContextOptions.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            dbContextOptions
                .UseMySql(mySqlConnectionString, sp.GetRequiredService<ServerVersion>())
                .UseExceptionProcessor();
        });

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddScoped<ReviewDbContextInitializer>();

        services.AddSingleton<ISerializer, SystemTextSerializer>();

        services.AddMemoryCache();
        services.AddSingleton<IMemoryCacheProvider, MemoryCacheProvider>();

        services.AddStackExchangeRedisCache(opt =>
        {
            opt.Configuration = configuration.GetConnectionString("Redis");
        });

        services.AddSingleton<IRedisCacheProvider, RedisCacheProvider>();
        services.AddSingleton<IRedisResilienceCacheProvider, RedisResilienceCacheProvider>();

        services.Configure<ArticleApiDefinitionOptions>(configuration.GetSection(ArticleApiDefinitionOptions.SectionName));
        AddApiClients(services, configuration);

        return services;
    }
    private static void AddApiClients(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();
        services.AddScoped<IArticleApiClientProvider, ArticleApiClientProvider>();
    }
}
