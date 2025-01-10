//using DataAccessLayer.Interface;

//namespace OptiTask.Services
//{
//    public class TaskScheduler : BackgroundService
//    {
//        private readonly ITaskAssignmentRepository _taskAssignmentRepository;
//        private readonly ITasksRepository _taskRepository;

//        private readonly ILogger _logger;

//        public TaskScheduler(ITasksRepository taskRepository, ILogger logger, ITaskAssignmentRepository taskAssignmentRepository)
//        {
//            _taskAssignmentRepository = taskAssignmentRepository;
//            _logger = logger;
//            _taskRepository = taskRepository;
//        }


//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            while (!stoppingToken.IsCancellationRequested)
//            {
//                try
//                {
//                    // Tüm görevleri al
//                    var assignments = await _taskAssignmentRepository.GetAll();

//                    foreach (var assignment in assignments)
//                    {
//                        var task = await _taskRepository.GetById(assignment.TaskId);
                        

//                        // Görevin tahmini süresi geçmiş mi kontrol et
//                        if (task.EstimatedLoad <= (DateTime.UtcNow - task.).TotalMinutes && task.Status != "Gecikti")
//                        {
//                            // Status alanını "Gecikti" olarak güncelle
//                            task.Status = "Gecikti";
//                            await _taskRepository.UpdateTaskAsync(task);

//                            _logger.LogInformation($"Görev {task.TaskId} gecikti olarak güncellendi.");
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError($"Hata oluştu: {ex.Message}");
//                }

//                // Görevi belirli aralıklarla çalıştır
//                await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
//            }
//        }
//    }
//}
