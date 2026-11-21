using System.Text.Json;
using Learniverse.Application.Interfaces.Common;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Learniverse.Infrastructure.Caching;

public sealed class RedisCacheService : ICacheService
{
    private const string TagVersionKeyPrefix = "cache:tag-version:";

    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(
        IDistributedCache distributedCache,
        ILogger<RedisCacheService> logger)
    {
        _distributedCache = distributedCache;
        _logger = logger;
    }

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        string tag,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan expiration,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(tag);
        ArgumentNullException.ThrowIfNull(factory);

        if (expiration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expiration),
                "Cache expiration must be greater than zero.");
        }

        string cacheKey;
        string? cachedJson;

        try
        {
            cacheKey = await GetVersionedCacheKeyAsync(
                key,
                tag,
                cancellationToken);

            cachedJson = await _distributedCache.GetStringAsync(
                cacheKey,
                cancellationToken);
        }
        catch (RedisException exception)
        {
            LogCacheFailure("read", exception);
            return await factory(cancellationToken);
        }

        if (cachedJson is not null)
        {
            try
            {
                var cachedValue = JsonSerializer.Deserialize<T>(cachedJson);

                if (cachedValue is not null)
                {
                    return cachedValue;
                }
            }
            catch (JsonException exception)
            {
                _logger.LogWarning(
                    "Redis cache entry could not be deserialized. Failure type: {FailureType}.",
                    exception.GetType().Name);
            }
        }

        var value = await factory(cancellationToken);

        try
        {
            await _distributedCache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(value),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration
                },
                cancellationToken);
        }
        catch (RedisException exception)
        {
            LogCacheFailure("write", exception);
        }

        return value;
    }

    public async Task InvalidateTagAsync(
        string tag,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tag);

        try
        {
            await _distributedCache.SetStringAsync(
                GetTagVersionKey(tag),
                Guid.NewGuid().ToString("N"),
                cancellationToken);
        }
        catch (RedisException exception)
        {
            LogCacheFailure("invalidation", exception);
        }
    }

    private async Task<string> GetVersionedCacheKeyAsync(
        string key,
        string tag,
        CancellationToken cancellationToken)
    {
        var tagVersionKey = GetTagVersionKey(tag);
        var version = await _distributedCache.GetStringAsync(
            tagVersionKey,
            cancellationToken);

        if (string.IsNullOrWhiteSpace(version))
        {
            version = Guid.NewGuid().ToString("N");
            await _distributedCache.SetStringAsync(
                tagVersionKey,
                version,
                cancellationToken);
        }

        return $"{key}:tag:{tag}:{version}";
    }

    private static string GetTagVersionKey(string tag)
    {
        return $"{TagVersionKeyPrefix}{tag}";
    }

    private void LogCacheFailure(string operation, RedisException exception)
    {
        _logger.LogWarning(
            "Redis cache {CacheOperation} failed. Continuing without the cache. Failure type: {FailureType}.",
            operation,
            exception.GetType().Name);
    }
}
