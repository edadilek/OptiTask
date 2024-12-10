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
        private readonly TaskService _taskService;
        private readonly ITasksRepository _taskRepository;

        public TaskController(TaskService taskService, ITasksRepository taskRepository)
        {
            _taskService = taskService;
            _taskRepository = taskRepository;
        }

        [HttpPost("assign-task/{taskId}")]
        public async Task<IActionResult> AssignTask(int taskId)
        {
            var task = await _taskRepository.GetById(taskId);

            if (task == null)
                return NotFound("Görev bulunamadı!");


            try
            {
                var assignedUser = await _taskService.AssignTaskAsync(task);
                return Ok(new
                {
                    Message = "Görev başarıyla atandı!",
                    AssignedTo = new { assignedUser.Id, assignedUser.Name }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}


//namespace OptiTask.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class TaskController : ControllerBase
//    {
//        private readonly ITasksRepository tasksRepository;
//        private readonly IUserRepository userRepository;
//        private readonly ITaskAssignmentRepository taskAssignmentRepository;


//        public TaskController(ITasksRepository context, IUserRepository userRepository, ITaskAssignmentRepository taskAssignmentRepositorye)
//        {
//            tasksRepository = context;
//            this.userRepository = userRepository;
//            this.taskAssignmentRepository = taskAssignmentRepository;
//        }

//        //[HttpPost]
//        //public async Task<IActionResult> AssignTask(int taskId, int userId, int estimatedTime, int difficulty)
//        //{
//        //    var userWorkload = await _workloadService.GetWorkloadAsync(userId);
//        //    double newLoad = userWorkload + (estimatedTime * difficulty);

//        //    await _workloadService.UpdateWorkloadAsync(userId, newLoad);

//        //    var taskAssignment = new TaskAssignment
//        //    {
//        //        TaskId = taskId,
//        //        UserId = userId,
//        //        AssignedAt = DateTime.UtcNow
//        //    };

//        //    _context.TaskAssignments.Add(taskAssignment);
//        //    await _context.SaveChangesAsync();

//        //    return Ok();
//        //}


//        // Yeni görev oluşturma
//        [HttpPost]
//        public async Task<IActionResult> CreateTask([FromBody] TasksDTO task)
//        {
//            Tasks newTask = new Tasks()
//            {
//                Description = task.Description,
//                Difficulty = task.Difficulty,
//                EstimatedLoad = task.EstimatedLoad,
//                Name = task.Name,
//                Role = task.Role,
//                Status = task.Status,
//            };

//            await tasksRepository.Create(newTask);

//            return Ok(task);
//        }


//        // Görevi kullanıcıya atama
//        [HttpPost("{taskId}/assign-user")]
//        public async Task<IActionResult> AssignTaskToUser(int taskId, [FromBody] int userId)
//        {
//            var task = await tasksRepository.GetById(taskId);
//            var user = await userRepository.GetById(userId);

//            if (task == null || user == null) return NotFound();

//            var assignment = new TaskAssignment
//            {
//                TaskId = taskId,
//                UserId = userId,
//                AssignedAt = DateTime.UtcNow
//            };

//            var result = await taskAssignmentRepository.Create(assignment);


//            return Ok(result);
//        }
//    }
//}








