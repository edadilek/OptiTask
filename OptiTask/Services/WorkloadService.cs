using Microsoft.EntityFrameworkCore.Storage;
using StackExchange.Redis;


namespace OptiTask.Services
{
    public class WorkloadService
    {
        private readonly StackExchange.Redis.IDatabase _redisDb;
        private readonly ILogger<WorkloadService> _logger;
        private const string WORKLOAD_KEY_PREFIX = "workload:";

        public WorkloadService(IConnectionMultiplexer redisConnection, ILogger<WorkloadService> logger)
        {
            _redisDb = redisConnection.GetDatabase();
            _logger = logger;
        }

        public async Task<double> GetWorkloadAsync(int userId)
        {
            var key = $"{WORKLOAD_KEY_PREFIX}{userId}";
            var workload = await _redisDb.StringGetAsync(key);
            return workload.HasValue ? double.TryParse(workload, out double result) ? result : 0.0 : 0.0;
        }

        public async Task IncreaseWorkloadAsync(int userId, double amount)
        {
            var key = $"{WORKLOAD_KEY_PREFIX}{userId}";
            var currentLoad = await GetWorkloadAsync(userId);
            await _redisDb.StringSetAsync(key, currentLoad + amount);
        }

        public async Task DecreaseWorkloadAsync(int userId, double amount)
        {
            var key = $"{WORKLOAD_KEY_PREFIX}{userId}";
            var currentLoad = await GetWorkloadAsync(userId);
            var newLoad = Math.Max(0, currentLoad - amount);
            await _redisDb.StringSetAsync(key, newLoad);
        }

        public async Task<List<string>> GetAllWorkloadKeys()
        {
            var keys = await _redisDb.ExecuteAsync("KEYS",$"{WORKLOAD_KEY_PREFIX}*");

            var result = ((RedisResult[]) keys).Select(k => k.ToString()).ToList();

            return result;
        }

    }
}

