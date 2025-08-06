using System.Text.Json;

namespace StudentGradesAPI.Services;

public interface ICacheManagerService
{
    Task<T?> GetFromCacheAsync<T>(string cacheKey);
    Task SetCacheAsync<T>(string cacheKey, T value, TimeSpan expiration);
}
