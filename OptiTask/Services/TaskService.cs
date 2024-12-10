using DataAccessLayer.Entity;
using DataAccessLayer.Interface;

namespace OptiTask.Services
{
    public class TaskService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITasksRepository _taskRepository;
        private readonly ITaskAssignmentRepository _taskAssignmentRepository;
        private readonly WorkloadService _workloadService;

        public TaskService(
            IUserRepository userRepository,
            ITasksRepository taskRepository,
            ITaskAssignmentRepository taskAssignmentRepository,
            WorkloadService workloadService)
        {
            _userRepository = userRepository;
            _taskRepository = taskRepository;
            _taskAssignmentRepository = taskAssignmentRepository;
            _workloadService = workloadService;
        }

        public async Task<List<User>> GetAvailableUsersAsync(string requiredRole, int teamId)
        {
            var users = await _userRepository.GetAll(user => user.Role == requiredRole && user.TeamId == teamId);
            return users.ToList();
        }

        public async Task<User> AssignTaskAsync(Task task)
        {
            var users = await GetAvailableUsersAsync(task.Role, task.TeamId);

            if (!users.Any())
                throw new InvalidOperationException("Görev için uygun kullanıcı bulunamadı!");

            User bestUser = null;
            double minWorkload = double.MaxValue;

            foreach (var user in users)
            {
                var currentWorkload = await _workloadService.GetWorkloadAsync(user.Id);
                if (currentWorkload < minWorkload)
                {
                    minWorkload = currentWorkload;
                    bestUser = user;
                }
            }

            if (bestUser == null)
                throw new InvalidOperationException("Kullanıcı atanamadı!");

            await AssignTaskToUserAsync(task, bestUser);
            return bestUser;
        }

        private async Task AssignTaskToUserAsync(Task task, User user)
        {
            var assignment = new TaskAssignment
            {
                TaskId = task.Id,
                UserId = user.Id,
                AssignedAt = DateTime.UtcNow
            };

            await _taskAssignmentRepository.Create(assignment);

            double taskLoad = task.EstimatedLoad * task.Difficulty;
            await _workloadService.IncreaseWorkloadAsync(user.Id, taskLoad);
        }
    }
}

