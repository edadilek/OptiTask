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
        private readonly ILogger<TaskController> _logger;

        public TaskController(TaskService taskService, ITasksRepository taskRepository, ILogger<TaskController> logger)
        {
            _taskService = taskService;
            _taskRepository = taskRepository;
            _logger = logger;
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TasksDTO taskDTO)
        {
            try
            {
                var createdTask = await _taskService.CreateTaskAsync(taskDTO);
                return Ok(createdTask);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                return StatusCode(500, "An error occurred while creating the task");
            }
        }

        // READ (All)
        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            try
            {
                var tasks = await _taskRepository.GetAll();
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tasks");
                return StatusCode(500, "An error occurred while retrieving tasks");
            }
        }

        // READ (By Id)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            try
            {
                var task = await _taskRepository.GetById(id);
                if (task == null)
                    return NotFound($"Task with ID {id} not found");

                return Ok(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving task");
                return StatusCode(500, "An error occurred while retrieving the task");
            }
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TasksDTO taskDTO)
        {
            try
            {
                var existingTask = await _taskRepository.GetById(id);
                if (existingTask == null)
                    return NotFound($"Task with ID {id} not found");

                existingTask.Name = taskDTO.Name;
                existingTask.Description = taskDTO.Description;
                existingTask.Role = taskDTO.Role;
                existingTask.Status = taskDTO.Status;
                existingTask.EstimatedLoad = taskDTO.EstimatedLoad;
                existingTask.Difficulty = taskDTO.Difficulty;

                var updatedTask = await _taskRepository.Update(existingTask);
                return Ok(updatedTask);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task");
                return StatusCode(500, "An error occurred while updating the task");
            }
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                var task = await _taskRepository.GetById(id);
                if (task == null)
                    return NotFound($"Task with ID {id} not found");

                await _taskRepository.Delete(task);
                return Ok($"Task with ID {id} was deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task");
                return StatusCode(500, "An error occurred while deleting the task");
            }
        }

        // Manuel atama
        [HttpPost("{taskId}/assign/{userId}")]
        public async Task<IActionResult> AssignTaskToUser(int taskId, int userId)
        {
            try
            {
                var assignment = await _taskService.AssignTaskToUserAsync(taskId, userId);
                return Ok(assignment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning task");
                return StatusCode(500, "An error occurred while assigning the task");
            }
        }

        // Otomatik atama
        [HttpPost("{taskId}/auto-assign/{teamId}")]
        public async Task<IActionResult> AutoAssignTask(int taskId, int teamId)
        {
            try
            {
                var assignment = await _taskService.AutoAssignTaskAsync(taskId, teamId);
                return Ok(assignment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error auto-assigning task");
                return StatusCode(500, "An error occurred while auto-assigning the task");
            }
        }
    }
}



//using Microsoft.AspNetCore.Mvc;
//using DataAccessLayer.Entity;
//using OptiTask.Services;
//using DataAccessLayer;
//using DataAccessLayer.Repository;
//using OptiTask.DTOs;
//using DataAccessLayer.Interface;

//namespace OptiTask.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class TaskController : ControllerBase
//    {
//        private readonly TaskService _taskService;
//        private readonly ITasksRepository _taskRepository;
//        private readonly ILogger<TaskController> _logger;

//        public TaskController(TaskService taskService, ITasksRepository taskRepository, ILogger<TaskController> logger)
//        {
//            _taskService = taskService;
//            _taskRepository = taskRepository;
//            _logger = logger;
//        }

//        [HttpPost]
//        public async Task<IActionResult> CreateTask([FromBody] TasksDTO taskDTO)
//        {
//            try
//            {
//                var createdTask = await _taskService.CreateTaskAsync(taskDTO);
//                return Ok(createdTask);
//            }
//            catch (InvalidOperationException ex)
//            {
//                return BadRequest(ex.Message);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error creating task");
//                return StatusCode(500, "An error occurred while creating the task");
//            }
//        }

//        // Manuel atama
//        [HttpPost("{taskId}/assign/{userId}")]
//        public async Task<IActionResult> AssignTaskToUser(int taskId, int userId)
//        {
//            try
//            {
//                var assignment = await _taskService.AssignTaskToUserAsync(taskId, userId);
//                return Ok(assignment);
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex.Message);
//            }

//        }

//        // Otomatik atama
//        [HttpPost("{taskId}/auto-assign/{teamId}")]
//        public async Task<IActionResult> AutoAssignTask(int taskId, int teamId)
//        {
//            try
//            {
//                var assignment = await _taskService.AutoAssignTaskAsync(taskId, teamId);
//                return Ok(assignment);
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex.Message);
//            }
//        }
//    }
//}