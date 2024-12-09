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

            var result = await tasksRepository.Create(newTask);

            return Ok(result);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var existedTask = await tasksRepository.GetById(id);
        
            if(existedTask == null) return NotFound();

            await tasksRepository.Delete(existedTask);

            return Ok("Task Silindi!");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TasksDTO tasksDTO)
        {
            var existedTask = await tasksRepository.GetById(id);

            if (existedTask == null) return NotFound();

            if (tasksDTO == null)
            {
                return BadRequest("Please provide a proper body!");
            }

            if (tasksDTO.Name != null)
            {
                existedTask.Name = tasksDTO.Name;
            }

            if (tasksDTO.Description != null)
            {
                existedTask.Description = tasksDTO.Description;
            }

            // Entitylerde Id dışındaki kısımları string olarak güncelle !!!

            var result = await tasksRepository.Update(existedTask);

            return Ok(result);
        }
    }
}



