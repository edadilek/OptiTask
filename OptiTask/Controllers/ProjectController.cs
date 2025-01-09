using Microsoft.AspNetCore.Mvc;
using DataAccessLayer.Entity;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer;
using DataAccessLayer.Repository;
using DataAccessLayer.Interface;
using OptiTask.DTOs;

namespace OptiTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectRepository projectRepository;
        private readonly ITeamRepository teamRepository;
        private readonly ITeamProjectRepository teamProjectRepository;

        public ProjectController(IProjectRepository projectRepository, ITeamRepository teamRepository, ITeamProjectRepository teamProjectRepository)
        {
            this.projectRepository = projectRepository;
            this.teamRepository = teamRepository;
            this.teamProjectRepository = teamProjectRepository;
        }

        // Projeleri listeleme
        [HttpGet]
        public async Task<IActionResult> GetAllProjects()
        {
            var projects = await projectRepository.GetAll();
            return Ok(projects);
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
        [HttpPost("{projectId}/assign-team")]
        public async Task<IActionResult> AssignTeamToProject(int projectId, [FromBody] int teamId)
        {
            var project = await projectRepository.GetById(projectId);
            var team = await teamRepository.GetById(teamId);

            if (project == null || team == null) return NotFound();

            var assignment = new TeamProject()
            {
                project = project,
                team = team,
                projectId = projectId,
                teamId = teamId
            };

            await teamProjectRepository.Create(assignment);
            return Ok(assignment);
        }
    }
}

