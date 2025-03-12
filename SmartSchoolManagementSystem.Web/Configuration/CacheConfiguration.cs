using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace SmartSchoolManagementSystem.Web.Configuration;

public static class CacheConfiguration
{
    public static IServiceCollection AddCustomCaching(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Memory Cache
        services.AddMemoryCache(options =>
        {
            options.SizeLimit = 1024; // Cache size limit in MB
            options.ExpirationScanFrequency = TimeSpan.FromMinutes(5);
        });

        // Add Distributed Cache (Redis)
        var redisSettings = configuration.GetSection("RedisSettings").Get<RedisSettings>();
        if (redisSettings?.Enabled == true)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisSettings.ConnectionString;
                options.InstanceName = redisSettings.InstanceName;
            });
        }
        else
        {
            // Fallback to distributed memory cache if Redis is not configured
            services.AddDistributedMemoryCache();
        }

        // Register cache service
        services.AddSingleton<ICacheService, CacheService>();

        return services;
    }
}

public class RedisSettings
{
    public bool Enabled { get; set; }
    public string ConnectionString { get; set; }
    public string InstanceName { get; set; }
}

public interface ICacheService
{
    T Get<T>(string key);
    void Set<T>(string key, T value, TimeSpan? expiration = null);
    void Remove(string key);
    bool Exists(string key);
}

public class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<CacheService> _logger;
    private readonly MemoryCacheEntryOptions _defaultCacheOptions;
    private readonly DistributedCacheEntryOptions _defaultDistributedCacheOptions;

    public CacheService(
        IMemoryCache memoryCache,
        IDistributedCache distributedCache,
        ILogger<CacheService> logger)
    {
        _memoryCache = memoryCache;
        _distributedCache = distributedCache;
        _logger = logger;

        _defaultCacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
            SlidingExpiration = TimeSpan.FromMinutes(30),
            Size = 1 // Size in MB
        };

        _defaultDistributedCacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
            SlidingExpiration = TimeSpan.FromMinutes(30)
        };
    }

    public T Get<T>(string key)
    {
        try
        {
            // Try memory cache first
            if (_memoryCache.TryGetValue(key, out T value))
            {
                _logger.LogDebug("Cache hit for key: {Key} (Memory Cache)", key);
                return value;
            }

            // Try distributed cache
            var data = _distributedCache.Get(key);
            if (data != null)
            {
                value = System.Text.Json.JsonSerializer.Deserialize<T>(data);
                
                // Add to memory cache for faster subsequent access
                _memoryCache.Set(key, value, _defaultCacheOptions);
                
                _logger.LogDebug("Cache hit for key: {Key} (Distributed Cache)", key);
                return value;
            }

            _logger.LogDebug("Cache miss for key: {Key}", key);
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving from cache for key: {Key}", key);
            return default;
        }
    }

    public void Set<T>(string key, T value, TimeSpan? expiration = null)
    {
        try
        {
            var memoryCacheOptions = new MemoryCacheEntryOptions(_defaultCacheOptions);
            var distributedCacheOptions = new DistributedCacheEntryOptions(_defaultDistributedCacheOptions);

            if (expiration.HasValue)
            {
                memoryCacheOptions.AbsoluteExpirationRelativeToNow = expiration;
                distributedCacheOptions.AbsoluteExpirationRelativeToNow = expiration;
            }

            // Set in memory cache
            _memoryCache.Set(key, value, memoryCacheOptions);

            // Set in distributed cache
            var data = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(value);
            _distributedCache.Set(key, data, distributedCacheOptions);

            _logger.LogDebug("Set cache for key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache for key: {Key}", key);
        }
    }

    public void Remove(string key)
    {
        try
        {
            _memoryCache.Remove(key);
            _distributedCache.Remove(key);
            _logger.LogDebug("Removed cache for key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache for key: {Key}", key);
        }
    }

    public bool Exists(string key)
    {
        try
        {
            // Check memory cache first
            if (_memoryCache.TryGetValue(key, out _))
            {
                return true;
            }

            // Check distributed cache
            var data = _distributedCache.Get(key);
            return data != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking cache existence for key: {Key}", key);
            return false;
        }
    }
}
