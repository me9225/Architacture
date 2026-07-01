using StackExchange.Redis;
using System.Text.Json;

namespace BsdFinalProject.Services
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
        Task RemoveByPatternAsync(string pattern);
        Task<bool> ExistsAsync(string key);
    }

    public class CacheService : ICacheService
    {
        private readonly IDatabase _db;
        private readonly IServer _server;
        private readonly int _defaultTtlSeconds;

        public CacheService(IConnectionMultiplexer redis, IConfiguration configuration)
        {
            _db = redis.GetDatabase();
            _server = redis.GetServer(redis.GetEndPoints().First());

            // קראו את ה-TTL מה-configuration
            _defaultTtlSeconds = int.Parse(configuration["Cache:DefaultTTL"] ?? "3600");
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _db.StringGetAsync(key);

            if (!value.HasValue)
                return default;

            try
            {
                return JsonSerializer.Deserialize<T>(value.ToString());
            }
            catch
            {
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var jsonValue = JsonSerializer.Serialize(value);

            // אם לא נקבע expiration, השתמש ב-default TTL
            var ttl = expiration ?? TimeSpan.FromSeconds(_defaultTtlSeconds);

            await _db.StringSetAsync(key, jsonValue, ttl);
        }

        public async Task RemoveAsync(string key)
        {
            await _db.KeyDeleteAsync(key);
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            var keys = _server.Keys(pattern: pattern).ToArray();
            if (keys.Length > 0)
            {
                await _db.KeyDeleteAsync(keys);
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            return await _db.KeyExistsAsync(key);
        }
    }
}
