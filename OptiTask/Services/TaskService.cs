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

    }
}

