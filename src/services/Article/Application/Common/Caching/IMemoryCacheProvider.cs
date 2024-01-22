namespace Application.Common.Caching;

public interface IMemoryCacheProvider
{
    T Get<T>(string key);
    void Set<T>(string key, T value);
    void Set<T>(string key, T value, CacheTime expiresIn);
    void Set<T>(string key, T value, int expiresInMinutes);
    void Remove(string key);
    void Clear();
}
