using System.Reflection;
using System.Text.Json;
using Application.Common.Caching;
using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Caching;

public class RedisCacheProvider(IDistributedCache cache) : IRedisCacheProvider
{
    private readonly string _keyPrefix = Assembly.GetEntryAssembly()?.GetName().Name;

    public T Get<T>(string key)
    {
        return GetAsync<T>(key).GetAwaiter().GetResult();
    }

    public async Task<T> GetAsync<T>(string key, CancellationToken token = default)
    {
        var value = await cache.GetAsync(BeatifyKey(key), token);

        var result = default(T);

        if (value == null)
            return result;

        T GenericDeserialize()
        {
            var readOnlySpan = new ReadOnlySpan<byte>(value);
            return JsonSerializer.Deserialize<T>(readOnlySpan);
        }

        return GenericDeserialize();
    }

    public async Task SetAsync<T>(string key, T value, CancellationToken token = default)
    {
        await SetInternalHelper(key, value, null, null, token);
    }

    public async Task SetAsync<T>(string key, T value, CacheTime expiresIn, CancellationToken token = default)
    {
        await SetInternalHelper(key, value, null, expiresIn, token);
    }

    public async Task SetAsync<T>(string key, T value, int expiresInMinutes, CancellationToken token = default)
    {
        await SetInternalHelper(key, value, expiresInMinutes, null, token);
    }

    public async Task RemoveAsync(string key, CancellationToken token = default)
    {
        await cache.RemoveAsync(BeatifyKey(key), token);
    }

    private string BeatifyKey(string key)
    {
        return $"{(string.IsNullOrWhiteSpace(_keyPrefix) ? "" : _keyPrefix + ":")}{key}";
    }

    private async Task SetInternalHelper<T>(
        string key,
        T value,
        int? expiresInMinutes,
        CacheTime? expiresIn,
        CancellationToken token = default)
    {
        var cacheEntryOptions = new DistributedCacheEntryOptions();

        if (expiresInMinutes.HasValue)
            cacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expiresInMinutes.Value);

        if (expiresIn.HasValue)
            cacheEntryOptions.AbsoluteExpirationRelativeToNow = expiresIn.Value.GetTimeSpanFromCacheTime();

        if (!expiresInMinutes.HasValue && !expiresIn.HasValue)
            cacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);

        var serializeToUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes(value);
        await cache.SetAsync(BeatifyKey(key), serializeToUtf8Bytes, cacheEntryOptions, token);
    }
}
