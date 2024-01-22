namespace Application.Common.Caching;

public interface IRedisCacheProvider
{
    Task<T> GetAsync<T>(string key, CancellationToken token = default);
    Task SetAsync<T>(string key, T value, CancellationToken token = default);
    Task SetAsync<T>(string key, T value, CacheTime expiresIn, CancellationToken token = default);
    Task SetAsync<T>(string key, T value, int expiresInMinutes, CancellationToken token = default);
    Task RemoveAsync(string key, CancellationToken token = default);
}
