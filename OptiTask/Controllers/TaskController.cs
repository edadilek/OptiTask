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
        private readonly IUserRepository _userRepository;
        private readonly ITaskAssignmentRepository _taskAssignmentRepository;

        public TaskController(TaskService taskService, ITasksRepository taskRepository, ILogger<TaskController> logger, IUserRepository userRepository, ITaskAssignmentRepository taskAssignmentRepository)
        {
            _taskService = taskService;
            _taskRepository = taskRepository;
            _logger = logger;
            _userRepository = userRepository;
            _taskAssignmentRepository = taskAssignmentRepository;
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
        [HttpPost("{id}/assign/{userId}")]
        public async Task<IActionResult> AssignTaskToUser(int id, int userId)
        {
            try
            {
                var assignment = await _taskService.AssignTaskToUserAsync(id, userId);
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
        [HttpPost("{id}/auto-assign/{teamId}")]
        public async Task<IActionResult> AutoAssignTask(int id, int teamId)
        {
            try
            {
                var assignment = await _taskService.AutoAssignTaskAsync(id, teamId);
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

        [HttpPut("{id}/done/{userId}")]
        public async Task<IActionResult> MarkDoneTask(int id, int userId)
        {
            var task = await _taskRepository.GetById(id);

            if (task == null)
            {
                return NotFound();
            }

            var user = await _userRepository.GetById(userId);

            if (user == null)
            {
                return NotFound();
            }

            var taskAssignment = await _taskAssignmentRepository.GetTaskAssignment(id, userId);

            if (taskAssignment == null)
            {
                return NotFound();
            }

            taskAssignment.Status = "Done";

            await _taskAssignmentRepository.Update(taskAssignment);

            return Ok();
        }
    }
}