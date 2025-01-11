using Microsoft.AspNetCore.Mvc;
using DataAccessLayer.Entity;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer;
using DataAccessLayer.Repository;
using DataAccessLayer.Interface;
using OptiTask.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace OptiTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectRepository projectRepository;
        private readonly ITeamRepository teamRepository;
        private readonly ITeamProjectRepository teamProjectRepository;
        private readonly ITasksRepository tasksRepository;
        private readonly IProjectTaskRepository projectTaskRepository;

        public ProjectController(IProjectRepository projectRepository, ITeamRepository teamRepository, ITeamProjectRepository teamProjectRepository, ITasksRepository tasksRepository, IProjectTaskRepository projectTaskRepository)
        {
            this.projectRepository = projectRepository;
            this.teamRepository = teamRepository;
            this.teamProjectRepository = teamProjectRepository;
            this.tasksRepository = tasksRepository;
            this.projectTaskRepository = projectTaskRepository;
        }

        // Projeleri listeleme
        [Authorize(Roles = "admin, project-manager")]
        [HttpGet]
        public async Task<IActionResult> GetAllProjects()
        {
            var projects = await projectRepository.GetAll();
            return Ok(projects);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetProjectById(int projectId)
        {
            var project = await projectRepository.GetById(projectId);

            if (project == null)
            {
                return NotFound("Project not found");
            }

            return Ok(project);
        }


        // Yeni proje oluşturma
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectDTO project)
        {
            var newProject = new Project()
            {
                Name = project.Name,
                Description = project.Description,
                Status = project.Status,
            };

            var result = await projectRepository.Create(newProject);

            return Ok(result);
        }

        // Projeyi takıma atama
        [HttpPost("{id}/assign-team")]
        public async Task<IActionResult> AssignTeamToProject(int id, [FromBody] int teamId)
        {
            var project = await projectRepository.GetById(id);
            var team = await teamRepository.GetById(teamId);

            if (project == null || team == null) return NotFound();

            var assignment = new TeamProject()
            {
                project = project,
                team = team,
                projectId = id,
                teamId = teamId
            };

            project.Status = "Assigned";

            await projectRepository.Update(project);

            await teamProjectRepository.Create(assignment);
            return Ok(assignment);
        }

        [HttpDelete("{id}/assign-team")]
        public async Task<IActionResult> ResignTeamFromProject(int id, [FromBody] int teamId)
        {
            var projectTeam = await teamProjectRepository.GetTeamProjectAsync(teamId, id);
            var project = await projectRepository.GetById(id);

            if(projectTeam == null) return NotFound();

            if (project.Status == "Assigned")
            {
                await teamProjectRepository.Delete(projectTeam);
                project.Status = "Idle";
            }

            if (project.Status == "Done")
            {
                await teamProjectRepository.Delete(projectTeam);
            }

            return Ok(projectTeam);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await projectRepository.GetById(id);

            if (project == null)
            {
                return NotFound("Proje bulunamadı");
            }

            await projectRepository.Delete(project);

            return Ok($"Proje Silindi: \n{project}");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] ProjectDTO projectDTO)
        {
            var project = await projectRepository.GetById(id);

            if (projectDTO.Status == null || projectDTO.Name == null || projectDTO.Description == null)
            {
                return BadRequest("Provide proper body");
            }

            project.Status = projectDTO.Status;
            project.Name = projectDTO.Name;
            project.Description = projectDTO.Description;

            await projectRepository.Update(project);

            return Ok(project);
        }

        [HttpPost("{id}/Task")]
        public async Task<IActionResult> AddTask(int id, [FromBody] int taskId)
        {
            var project = await projectRepository.GetById(id);

            if (project == null)
            {
                return NotFound("Project not found");
            }

            var task = await tasksRepository.GetById(taskId);

            if (task == null)
            {
                return NotFound("Task not found");
            }

            var projectTask = new ProjectTask()
            {
                projectId = project.ProjectId,
                taskId = task.TaskId
            };

            var pt = await projectTaskRepository.Create(projectTask);

            return Ok(pt);
        }

        [HttpDelete("{id}/Task")]
        public async Task<IActionResult> RemoveTask(int id, [FromBody] int taskId)
        {
            var project = await projectRepository.GetById(id);

            if (project == null)
            {
                return NotFound("Project not found");
            }

            var task = await tasksRepository.GetById(taskId);

            if (task == null)
            {
                return NotFound("Task not found");
            }

            var projectTask = await projectTaskRepository.GetProjectTask(id, taskId);

            await projectTaskRepository.Delete(projectTask);

            return Ok(projectTask);

        }
        
    }
}

