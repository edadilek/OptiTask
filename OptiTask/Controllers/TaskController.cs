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

        // Manuel atama
        [HttpPost("{taskId}/assign/{userId}")]
        public async Task<IActionResult> AssignTaskToUser(int taskId, int userId)
        {
            try
            {
                var assignment = await _taskService.AssignTaskToUserAsync(taskId, userId);
                return Ok(assignment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}



