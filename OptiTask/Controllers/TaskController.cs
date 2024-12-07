using Microsoft.AspNetCore.Mvc;
using DataAccessLayer.Entity;
using OptiTask.Services;
using DataAccessLayer;
using DataAccessLayer.Repository;
using OptiTask.DTOs;
using DataAccessLayer.Interface;

namespace OptiTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITasksRepository tasksRepository;
        private readonly IUserRepository userRepository;
        private readonly ITaskAssignmentRepository taskAssignmentRepository;

        public TaskController(ITasksRepository context, IUserRepository userRepository, ITaskAssignmentRepository taskAssignmentRepository)
        {
            tasksRepository = context;
            this.userRepository = userRepository;
            this.taskAssignmentRepository = taskAssignmentRepository;
        }

        // Yeni görev oluşturma
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TasksDTO task)
        {
            Tasks newTask = new Tasks()
            {
                Description = task.Description,
                Difficulty = task.Difficulty,
                EstimatedLoad = task.EstimatedLoad,
                Name = task.Name,
                Role = task.Role,
                Status = task.Status,
            };

            await tasksRepository.Create(newTask);

            return Ok(task);
        }


        // Görevi kullanıcıya atama
        [HttpPost("{taskId}/assign-user")]
        public async Task<IActionResult> AssignTaskToUser(int taskId, [FromBody] int userId)
        {
            var task = await tasksRepository.GetById(taskId);
            var user = await userRepository.GetById(userId);

            if (task == null || user == null) return NotFound();

            var assignment = new TaskAssignment
            {
                TaskId = taskId,
                UserId = userId,
                AssignedAt = DateTime.UtcNow
            };

            var result = await taskAssignmentRepository.Create(assignment);
            

            return Ok(result);
        }
    }
}



