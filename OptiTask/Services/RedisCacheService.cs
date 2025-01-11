using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text.Json;

namespace OptiTask.Services
{
    public class RedisCacheService
    {
        private readonly IDatabase _redisDb;
        private readonly AppDbContext _dbContext;
        private readonly ILogger<RedisCacheService> _logger;
        private const string KEY_PREFIX = "optitask:";

        public RedisCacheService(
            IConnectionMultiplexer redis,
            AppDbContext dbContext,
            ILogger<RedisCacheService> logger)
        {
            _redisDb = redis.GetDatabase();
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task InitializeCacheAsync()
        {
            try
            {
                _logger.LogInformation("Starting cache initialization");

                // Önce tüm cache'i temizle
                await ClearAllCacheAsync();

                var workloads = await _dbContext.Workloads.ToListAsync();

                foreach (var workload in workloads)
                {
                    await _redisDb.StringSetAsync(
    $"{KEY_PREFIX}team:{workload.UserId}",
    JsonSerializer.Serialize(workload)
);
                }

                _logger.LogInformation("Cache initialization completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during cache initialization");
                throw;
            }
        }

        private async Task ClearAllCacheAsync()
        {
            var server = _redisDb.Multiplexer.GetServer(_redisDb.Multiplexer.GetEndPoints().First());
            var keys = server.Keys(pattern: $"{KEY_PREFIX}*");
            foreach (var key in keys)
            {
                await _redisDb.KeyDeleteAsync(key);
            }
        }

        public async Task<T> GetFromCacheAsync<T>(string key) where T : class
        {
            var value = await _redisDb.StringGetAsync($"{KEY_PREFIX}{key}");
            if (!value.HasValue)
                return null;

            return JsonSerializer.Deserialize<T>(value);
        }

        public async Task SetToCacheAsync<T>(string key, T value)
        {
            await _redisDb.StringSetAsync(
                $"{KEY_PREFIX}{key}",
                JsonSerializer.Serialize(value)
            );
        }

        public async Task RemoveFromCacheAsync(string key)
        {
            await _redisDb.KeyDeleteAsync($"{KEY_PREFIX}{key}");
        }
    }
}