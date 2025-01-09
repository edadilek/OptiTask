using Microsoft.EntityFrameworkCore.Storage;
using StackExchange.Redis;

namespace OptiTask.Services
{
    //public class WorkloadService
    //{
    //    private readonly IConnectionMultiplexer _redis;

    //    public WorkloadService(IConnectionMultiplexer redis)
    //    {
    //        _redis = redis;
    //    }

    //    public async Task<double> GetWorkloadAsync(int userId)
    //    {
    //        var db = _redis.GetDatabase();
    //        string key = $"workload:{userId}";
    //        var workload = await db.StringGetAsync(key);
    //        return workload.HasValue ? double.Parse(workload) : 0;
    //    }

    //    public async Task UpdateWorkloadAsync(int userId, double newLoad)
    //    {
    //        var db = _redis.GetDatabase();
    //        string key = $"workload:{userId}";
    //        await db.StringSetAsync(key, newLoad);
    //    }
    //}

    public class WorkloadService
    {
        private readonly StackExchange.Redis.IDatabase _redisDb;

        public WorkloadService(IConnectionMultiplexer redisConnection)
        {
            // Redis bağlantısını alıp kullanılabilir hale getiriyoruz.
            _redisDb = redisConnection.GetDatabase();
        }

        /// <summary>
        /// Kullanıcının current_load değerini getirir.
        /// </summary>
        public async Task<double> GetWorkloadAsync(int userId)
        {
            var workload = await _redisDb.StringGetAsync($"workload:{userId}");
            return workload.HasValue ? double.Parse(workload) : 0.0; // Eğer yoksa 0.0 döner.
        }

        // Kullanıcının iş yükünü güncelle
        public async Task UpdateWorkloadAsync(int userId, double newWorkload)
        {
            await _redisDb.StringSetAsync($"user:{userId}:workload", newWorkload);
        }

        /// <summary>
        /// Kullanıcının iş yükünü (current_load) ayarlar.
        /// </summary>
        public async Task SetWorkloadAsync(int userId, double currentLoad)
        {
            await _redisDb.StringSetAsync($"workload:{userId}", currentLoad);
        }

        /// <summary>
        /// Kullanıcının iş yükünü azaltır.
        /// </summary>
        public async Task DecreaseWorkloadAsync(int userId, double amount)
        {
            double currentLoad = await GetWorkloadAsync(userId);
            currentLoad = Math.Max(0, currentLoad - amount); // İş yükü negatif olmasın.
            await SetWorkloadAsync(userId, currentLoad);
        }

        /// <summary>
        /// Kullanıcının iş yükünü artırır.
        /// </summary>
        public async Task IncreaseWorkloadAsync(int userId, double amount)
        {
            double currentLoad = await GetWorkloadAsync(userId);
            currentLoad += amount;
            await SetWorkloadAsync(userId, currentLoad);
        }
    }
}

