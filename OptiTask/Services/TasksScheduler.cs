using DataAccessLayer.Interface;
using DataAccessLayer.Repository;
using System.Threading;

namespace OptiTask.Services
{
    //taskın zamanı geçip geçmediğinin kontrolü için servis 
    //taskin atama tarihine bakıp (taskassignments tablosundan - assigned_at) bu zamana
    //taskın estimated time ekleyip şu anki zamanla kıyaslıyoruz
    public class TasksScheduler : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly ILogger<TasksScheduler> _logger;

        public TasksScheduler(ILogger<TasksScheduler> logger, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var _taskAssignmentRepository = scope.ServiceProvider.GetRequiredService<ITaskAssignmentRepository>();
                        var _tasksRepository = scope.ServiceProvider.GetRequiredService<ITasksRepository>();

                        _logger.LogInformation("Starting task delay check at: {time}", DateTimeOffset.Now);

                        var assignments = await _taskAssignmentRepository.GetAll();

                        foreach (var assignment in assignments)
                        {
                            var task = await _tasksRepository.GetById(assignment.TaskId);

                            if (task.Status != "Done" && task.EstimatedLoad <= (DateTime.UtcNow - assignment.AssignedAt).TotalMinutes && assignment.Status != "Delayed")
                            {
                                assignment.Status = "Delayed";

                                await _taskAssignmentRepository.Update(assignment);
                            }
                        }
                        _logger.LogInformation("Completed task delay check at: {time}", DateTimeOffset.Now);

                        await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
                    }
                    }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while checking task delays");
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                }
            }
        }
    }
}
