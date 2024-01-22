using System.Collections.Concurrent;

namespace Application.Common.Caching;

public static class CacheExtensions
{
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> KeyedSemaphoreDictionary = new();

    public static async Task<T> GetAsync<T>(
        this IRedisCacheProvider cacheProvider,
        string key,
        int cacheTime,
        Func<Task<T>> acquire,
        CancellationToken token = default)
    {
        var value = await cacheProvider.GetAsync<T>(key, token);

        if (value != null) return value;
        {
            var keyedSemaphoreSlim = KeyedSemaphoreDictionary.GetOrAdd(key, s => new SemaphoreSlim(1, 1));

            await keyedSemaphoreSlim.WaitAsync(token);

            try
            {
                value = await cacheProvider.GetAsync<T>(key, token);

                if (value != null) return value;
                {
                    value = await acquire();

                    if (cacheTime > 0) await cacheProvider.SetAsync(key, value, cacheTime, token);
                }
            }
            finally
            {
                keyedSemaphoreSlim.Release();
                KeyedSemaphoreDictionary.TryRemove(key, out _);
            }
        }

        return value;
    }

    public static async Task<T> GetAsync<T>(
        this IRedisCacheProvider cacheProvider,
        string key,
        Func<Task<T>> acquire,
        CancellationToken token = default)
    {
        return await cacheProvider.GetAsync(key, CacheTime.None, acquire, token);
    }

    public static async Task<T> GetAsync<T>(
        this IRedisCacheProvider cacheProvider,
        string key,
        CacheTime cacheTime,
        Func<Task<T>> acquire,
        CancellationToken token = default)
    {
        var minutes = cacheTime.GetTimeSpanFromCacheTime().TotalMinutes;

        return await cacheProvider.GetAsync(key, (int)minutes, acquire, token);
    }

    public static TimeSpan GetTimeSpanFromCacheTime(this CacheTime cacheTime) =>
        cacheTime switch
        {
            CacheTime.OneMinute => TimeSpan.FromMinutes(1),
            CacheTime.FifteenMinutes => TimeSpan.FromMinutes(15),
            CacheTime.ThirtyMinutes => TimeSpan.FromMinutes(30),
            CacheTime.OneHour => TimeSpan.FromHours(1),
            CacheTime.ThreeHours => TimeSpan.FromHours(3),
            CacheTime.SixHours => TimeSpan.FromHours(6),
            CacheTime.TwelveHours => TimeSpan.FromHours(12),
            CacheTime.OneDay => TimeSpan.FromDays(1),
            CacheTime.OneWeek => TimeSpan.FromDays(7),
            CacheTime.OneMonth => TimeSpan.FromDays(30),
            CacheTime.OneYear => TimeSpan.FromDays(365),
            _ => TimeSpan.FromMinutes(15)
        };
}
