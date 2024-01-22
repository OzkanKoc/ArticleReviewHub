using Application.Common.Caching;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Wrap;

namespace Infrastructure.Caching;

public class RedisResilienceCacheProvider : IRedisResilienceCacheProvider
{
    private readonly IRedisCacheProvider _redisCacheProvider;
    private readonly IMemoryCacheProvider _memoryCacheProvider;
    private readonly ILogger<RedisResilienceCacheProvider> _logger;
    private readonly AsyncPolicyWrap _policyCache;

    public RedisResilienceCacheProvider(
        IRedisCacheProvider redisCacheProvider,
        IMemoryCacheProvider memoryCacheProvider,
        ILogger<RedisResilienceCacheProvider> logger)
    {
        _redisCacheProvider = redisCacheProvider;
        _memoryCacheProvider = memoryCacheProvider;
        _logger = logger;

        //Setting a circuit breaker policy
        var circuitBreakerPolicy = Policy.Handle<Exception>()
            .CircuitBreakerAsync(
                5,
                TimeSpan.FromMinutes(1),
                (exception, circuitState, timeSpan, context) =>
                {
                    _logger.LogError(exception,
                        $"{nameof(RedisResilienceCacheProvider)} Circuit breaker cut, requests don't flow for {timeSpan} minutes!");
                },
                context =>
                {
                    _logger.LogInformation(
                        $"{nameof(RedisResilienceCacheProvider)} Circuit breaker closed, requests flow normally!");
                },
                () =>
                {
                    _logger.LogInformation(
                        $"{nameof(RedisResilienceCacheProvider)} Circuit breaker in test mode, one request will be allowed.");
                });

        //Setting a timeout policy
        var timeoutPolicy = Policy.TimeoutAsync(3,
            (context, timeSpan, task) =>
            {
                _logger.LogWarning("Timeout delegate fired after {TotalMilliseconds}", timeSpan.TotalMilliseconds);
                return Task.CompletedTask;
            });

        //Wrapping of timeout policy and circuit breaker policy
        _policyCache = timeoutPolicy.WrapAsync(circuitBreakerPolicy);
    }

    public async Task<T> GetAsync<T>(string key, CancellationToken token = default)
    {
        var memoryCacheItem = _memoryCacheProvider.Get<T>(key);

        if (memoryCacheItem is not null) return memoryCacheItem;

        try
        {
            var redisCacheItem = await _policyCache.ExecuteAsync(
                async ct => await _redisCacheProvider.GetAsync<T>(key, ct), token);

            if (redisCacheItem is not null)
                _memoryCacheProvider.Set(key, redisCacheItem, 5);

            return redisCacheItem;
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, $"{nameof(RedisResilienceCacheProvider)} Circuit breaker open!");
            return default;
        }
    }

    public Task SetAsync<T>(string key, T value, CancellationToken token = default)
    {
        return SetInternalHelper(key, value, null, null, token);
    }

    public Task SetAsync<T>(string key, T value, CacheTime expiresIn, CancellationToken token = default)
    {
        return SetInternalHelper(key, value, null, expiresIn, token);
    }

    public Task SetAsync<T>(string key, T value, int expiresInMinutes, CancellationToken token = default)
    {
        return SetInternalHelper(key, value, expiresInMinutes, null, token);
    }

    public async Task RemoveAsync(string key, CancellationToken token = default)
    {
        _memoryCacheProvider.Remove(key);

        try
        {
            await _policyCache.ExecuteAsync(
                async ct => await _redisCacheProvider.RemoveAsync(key, ct), token);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, $"{nameof(RedisResilienceCacheProvider)} Circuit breaker open!");
        }
    }

    private async Task SetInternalHelper<T>(
        string key,
        T value,
        int? expiresInMinutes,
        CacheTime? expiresIn,
        CancellationToken token = default)
    {
        _memoryCacheProvider.Set(key, value, 1);

        try
        {
            if (expiresInMinutes.HasValue)
                await _policyCache.ExecuteAsync(
                    async ct =>
                        await _redisCacheProvider.SetAsync(key, value, expiresInMinutes.Value, ct), token);

            if (expiresIn.HasValue)
                await _policyCache.ExecuteAsync(
                    async ct =>
                        await _redisCacheProvider.SetAsync(key, value, expiresIn.Value, ct), token);

            if (!expiresInMinutes.HasValue && !expiresIn.HasValue)
                await _policyCache.ExecuteAsync(
                    async ct =>
                        await _redisCacheProvider.SetAsync(key, value, ct), token);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, $"{nameof(RedisResilienceCacheProvider)} Circuit breaker open!");
        }
    }
}
