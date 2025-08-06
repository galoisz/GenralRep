using StackExchange.Redis;
using System.Text.Json;

namespace StudentGradesAPI.Services;

public class CacheManagerService : ICacheManagerService
{
    private readonly IDatabase _redisDb;

    public CacheManagerService(IConnectionMultiplexer redis)
    {
        _redisDb = redis.GetDatabase();
    }

    public async Task<T?> GetFromCacheAsync<T>(string cacheKey)
    {
        var cachedData = await _redisDb.StringGetAsync(cacheKey);
        if (cachedData.IsNullOrEmpty) return default;

        return JsonSerializer.Deserialize<T>(cachedData);
    }

    public async Task SetCacheAsync<T>(string cacheKey, T value, TimeSpan expiration)
    {
        var serializedData = JsonSerializer.Serialize(value);
        await _redisDb.StringSetAsync(cacheKey, serializedData, expiration);
    }
}
