public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value);
    Task RemoveAsync(string key);
    Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> loadFromDb);
}
