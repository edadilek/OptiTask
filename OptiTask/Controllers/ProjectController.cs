using Microsoft.AspNetCore.Mvc;
using DataAccessLayer.Entity;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer;
using DataAccessLayer.Repository;
using DataAccessLayer.Interface;

namespace OptiTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectRepository projectRepository;
        private readonly ITeamRepository teamRepository;

        public ProjectController(IProjectRepository projectRepository, ITeamRepository teamRepository)
        {
            this.projectRepository = projectRepository;
            this.teamRepository = teamRepository;
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
        public async Task<IActionResult> CreateProject([FromBody] Project project)
        {
            var newProject = new Project() {
                Description = project.Description,
                Name = project.Name,
                Status = project.Status,
                Team = project.Team,
                TeamId = project.TeamId,
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

            project.TeamId = teamId;

            project.Team = team;
            project.TeamId = teamId;

            await projectRepository.Update(project);
            return Ok(project);
        }
    }
}

