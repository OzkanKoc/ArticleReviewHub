using Application.Common.Caching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Infrastructure.Caching;

public class MemoryCacheProvider(IMemoryCache memoryCache) : IMemoryCacheProvider
{
    private CancellationTokenSource _cacheCancellationToken = new();

    public T Get<T>(string key)
    {
        return memoryCache.Get<T>(key);
    }

    public void Set<T>(string key, T value)
    {
        SetInternalHelper(key, value, null, null);
    }

    public void Set<T>(string key, T value, CacheTime expiresIn)
    {
        SetInternalHelper(key, value, null, expiresIn);
    }

    public void Set<T>(string key, T value, int expiresInMinutes)
    {
        SetInternalHelper(key, value, expiresInMinutes, null);
    }

    public void Remove(string key)
    {
        memoryCache.Remove(key);
    }

    public void Clear()
    {
        _cacheCancellationToken.Cancel();

        _cacheCancellationToken.Dispose();
        _cacheCancellationToken = new CancellationTokenSource();
    }

    private void SetInternalHelper<T>(
        string key,
        T value,
        int? expiresInMinutes,
        CacheTime? expiresIn)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions();

        if (expiresInMinutes.HasValue)
            cacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expiresInMinutes.Value);

        if (expiresIn.HasValue)
            cacheEntryOptions.AbsoluteExpirationRelativeToNow = expiresIn.Value.GetTimeSpanFromCacheTime();

        if (!expiresInMinutes.HasValue && !expiresIn.HasValue)
            cacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);

        cacheEntryOptions.AddExpirationToken(new CancellationChangeToken(_cacheCancellationToken.Token));

        memoryCache.Set(key, value, cacheEntryOptions);
    }
}
