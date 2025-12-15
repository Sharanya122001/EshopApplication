using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    public CacheService(IDistributedCache cache)
    {
        _cache = cache;
    }
    public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> loadFromDb)
    {
        var cached = await GetAsync<T>(key);
        if (cached != null)
            return cached;

        var fresh = await loadFromDb();
        await SetAsync(key, fresh);
        return fresh;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var json = await _cache.GetStringAsync(key);
        return json == null ? default : JsonSerializer.Deserialize<T>(json);
    }

    public async Task SetAsync<T>(string key, T value)
    {
        var json = JsonSerializer.Serialize(value);

        await _cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        });
    }
    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }
}
