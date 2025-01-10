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
    }
}