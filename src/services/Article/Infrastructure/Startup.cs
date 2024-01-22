using Application.Common.Repositories;
using EntityFramework.Exceptions.MySQL.Pomelo;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application.Common.Caching;
using Infrastructure.Caching;

namespace Infrastructure;

public static class Startup
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        
        var mySqlConnectionString = configuration.GetConnectionString("DatabaseConnection");

        services.AddSingleton(_ => ServerVersion.AutoDetect(mySqlConnectionString));

        services.AddDbContext<ArticleDbContext>((sp, dbContextOptions) =>
        {
            dbContextOptions.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            dbContextOptions
                .UseMySql(mySqlConnectionString, sp.GetRequiredService<ServerVersion>())
                .UseExceptionProcessor();
        });

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddScoped<ArticleDbContextInitializer>();

        services.AddMemoryCache();
        services.AddSingleton<IMemoryCacheProvider, MemoryCacheProvider>();

        services.AddStackExchangeRedisCache(opt =>
        {
            opt.Configuration = configuration.GetConnectionString("Redis");
        });

        services.AddSingleton<IRedisCacheProvider, RedisCacheProvider>();
        services.AddSingleton<IRedisResilienceCacheProvider, RedisResilienceCacheProvider>();

        return services;
    }
}
