using DataAccessLayer.Interface;
using System.Threading;

namespace OptiTask.Services
{
    public class TaskScheduler : BackgroundService
    {
        private readonly ITaskAssignmentRepository _taskAssignmentRepository;
        private readonly ITasksRepository _taskRepository;

        private readonly ILogger _logger;

        public TaskScheduler(ITasksRepository taskRepository, ILogger logger, ITaskAssignmentRepository taskAssignmentRepository)
        {
            _taskAssignmentRepository = taskAssignmentRepository;
            _logger = logger;
            _taskRepository = taskRepository;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var assignments = await _taskAssignmentRepository.GetAll();

                    foreach (var assignment in assignments)
                    {
                        var task = await _taskRepository.GetById(assignment.TaskId);

                        if (task.EstimatedLoad <= (DateTime.UtcNow - assignment.AssignedAt).TotalMinutes && assignment.Status != "Delayed")
                        {
                            assignment.Status = "Delayed";

                            await _taskAssignmentRepository.Update(assignment);
                        }
                    }
                    _logger.LogInformation("Assignment Delay Check");

                    await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"An Error Occured: {ex.Message}");
                }
            }
        }
    }
}
