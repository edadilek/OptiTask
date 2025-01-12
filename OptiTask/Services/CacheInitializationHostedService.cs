namespace OptiTask.Services
{
    //postgredeki verileri redise atıp cache gibi kullanmak için 
    //uygulama başlatıldığında Redis önbelleğini (cache) doldurmak için kullanılan Hosted Service
    public class CacheInitializationHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CacheInitializationHostedService> _logger;

        public CacheInitializationHostedService(
            IServiceProvider serviceProvider,
            ILogger<CacheInitializationHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var cacheService = scope.ServiceProvider.GetRequiredService<RedisCacheService>();

            try
            {
                await cacheService.InitializeCacheAsync();
                _logger.LogInformation("Cache initialization completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cache initialization failed");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    }
}
