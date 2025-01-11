using DataAccessLayer.Entity;
using DataAccessLayer.Interface;
using OptiTask.DTOs;
using OptiTask.Middlewares;

namespace OptiTask.Services
{
    public class TaskService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITasksRepository _taskRepository;
        private readonly ITaskAssignmentRepository _taskAssignmentRepository;
        private readonly ITeamMemberRepository _teamMemberRepository;
        private readonly IWorkloadRepository _workloadRepository;
        private readonly WorkloadService _workloadService;
        private readonly ILogger<TaskService> _logger;

        public TaskService(
            IUserRepository userRepository,
            ITasksRepository taskRepository,
            ITaskAssignmentRepository taskAssignmentRepository,
            ITeamMemberRepository teamMemberRepository,
            IWorkloadRepository workloadRepository,
            WorkloadService workloadService,
            ILogger<TaskService> logger)
        {
            _userRepository = userRepository;
            _taskRepository = taskRepository;
            _taskAssignmentRepository = taskAssignmentRepository;
            _teamMemberRepository = teamMemberRepository;
            _workloadRepository = workloadRepository;
            _workloadService = workloadService;
            _logger = logger;
        }

        //Task oluşturma
        public async Task<Tasks> CreateTaskAsync(TasksDTO taskDTO)
        {
            // İş yükü ve zorluk derecesi kontrolü
            if (taskDTO.EstimatedLoad <= 0 || taskDTO.Difficulty <= 0)
            {
                throw new InvalidOperationException("Estimated load and difficulty must be greater than zero.");
            }

            var newTask = new Tasks
            {
                Name = taskDTO.Name,
                Description = taskDTO.Description,
                Role = taskDTO.Role,
                Status = "Idle",
                EstimatedLoad = taskDTO.EstimatedLoad,
                Difficulty = taskDTO.Difficulty
            };

            return await _taskRepository.Create(newTask);
        }

        // Manuel atama
        public async Task<TaskAssignment> AssignTaskToUserAsync(int taskId, int userId)
        {
            var task = await _taskRepository.GetById(taskId);
            var user = await _userRepository.GetById(userId);

            if (task == null || user == null)
                throw new NotFoundException("Task or user not found");

            if (task.Status != "Idle")
            {
                throw new InvalidOperationException("This task has already been assigned");
            }

            // Rol kontrolü
            if (user.Role != task.Role)
                throw new InvalidOperationException("User role does not match task requirements");

            var workload = CalculateTaskWorkload(task);
            await _workloadService.IncreaseWorkloadAsync(userId, workload);

            var workloadDb = new Workload()
            {
                userId = userId,
                workload = workload
            };
            var dbRes = await _workloadRepository.Create(workloadDb);
            
            if (dbRes == null)
            {
                _logger.LogError("Workload registration failed!");
                throw new Exception("Workload registration failed");
            }

            var assignment = new TaskAssignment
            {
                TaskId = taskId,
                UserId = userId,
                AssignedAt = DateTime.UtcNow,
                Status = "In Processing"
            };

            return await _taskAssignmentRepository.Create(assignment);
        }

        // Otomatik atama
        public async Task<TaskAssignment> AutoAssignTaskAsync(int taskId, int teamId)
        {
            var task = await _taskRepository.GetById(taskId);
            if (task == null)
                throw new NotFoundException("Task not found");
            if (task.Status != "Idle")
            {
                throw new InvalidOperationException("This task has already been assigned");
            }

            // Takımdaki uygun role sahip kullanıcıları bul
            var teamMembers = await _teamMemberRepository.GetTeamMembers(teamId);
            if (teamMembers == null || !teamMembers.Any())
                throw new InvalidOperationException("No team members found");

            var eligibleUsers = teamMembers
                .Where(tm => tm.User?.Role == task.Role)
                .ToList();

            if (!eligibleUsers.Any())
                throw new InvalidOperationException("No eligible users found for this task");

            // Kullanıcıların iş yüklerini al
            var userWorkloads = new Dictionary<int, double>();
            foreach (var member in eligibleUsers)
            {
                try
                {
                    var workload = await _workloadService.GetWorkloadAsync(member.UserId);
                    userWorkloads[member.UserId] = workload;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting workload for user {UserId}", member.UserId);
                    userWorkloads[member.UserId] = 0.0; // Hata durumunda varsayılan değer
                }
            }

            // En uygun kullanıcıyı seç
            var selectedUserId = await SelectBestUserForTaskAsync(userWorkloads);

            //task.Status = "In Processing";

            // Seçilen kullanıcıya görevi ata
            return await AssignTaskToUserAsync(taskId, selectedUserId);
        }

        private async Task<int> SelectBestUserForTaskAsync(Dictionary<int, double> userWorkloads)
        {
            await Task.CompletedTask; // Eğer gerçekten asenkron işlem yapılmıyorsa

            if (userWorkloads.Values.All(w => w == 0))
            {
                return userWorkloads.Keys.OrderBy(x => Guid.NewGuid()).First();
            }

            var minWorkload = userWorkloads.Min(w => w.Value);
            var usersWithMinWorkload = userWorkloads
                .Where(w => w.Value == minWorkload)
                .Select(w => w.Key)
                .ToList();

            return usersWithMinWorkload.Count > 1
                ? usersWithMinWorkload[Random.Shared.Next(usersWithMinWorkload.Count)]
                : usersWithMinWorkload.First();
        }

        private double CalculateTaskWorkload(Tasks task)
        {
            return task.EstimatedLoad * task.Difficulty;
        }
    } 
}

