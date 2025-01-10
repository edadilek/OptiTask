using Microsoft.AspNetCore.Mvc;
using DataAccessLayer.Entity;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer;
using DataAccessLayer.Repository;
using OptiTask.DTOs;
using DataAccessLayer.Interface;

namespace OptiTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamController : ControllerBase
    {
        private readonly ITeamRepository teamRepository;
        private readonly IUserRepository userRepository;
        private readonly ITeamMemberRepository teamMemberRepository;

        public TeamController(ITeamRepository context, IUserRepository userRepository, ITeamMemberRepository teamMemberRepository)
        {
            teamRepository = context;
            this.userRepository = userRepository;
            this.teamMemberRepository = teamMemberRepository;
        }

        // Takımları listeleme
        [HttpGet]
        public async Task<IActionResult> GetAllTeams()
        {
            var teams = await teamRepository.GetAll();
            return Ok(teams);
        }

        [HttpGet("{teamId}")]
        public async Task<IActionResult> GetTeamById(int teamId)
        {
            var team = await teamRepository.GetById(teamId);

            if (team == null)
            {
                return NotFound();
            }

            return Ok(team);
        }

        // Yeni takım oluşturma
        [HttpPost]
        public async Task<IActionResult> CreateTeam([FromBody] TeamDTO team)
        {
            var newTeam = new Team()
            {
                Description = team.Description,
                Name = team.Name,
            };
            await teamRepository.Create(newTeam);
            return Ok(team);
        }

        [HttpDelete("{teamId}")]
        public async Task<IActionResult> DeleteTeam(int teamId)
        {
            var team = await teamRepository.GetById(teamId);

            if (team == null)
            {
                return NotFound();
            }
        
            await teamRepository.Delete(team);

            return Ok(team);
        }

        [HttpPut("{teamId}")]
        public async Task<IActionResult> UpdateTeam(int teamId, [FromBody] TeamDTO team)
        {
            var existedTeam = await teamRepository.GetById(teamId);

            if (existedTeam == null)
            {
                return NotFound("Team was not found!");
            }

            if (team.Name == null || team.Description == null)
            {
                return BadRequest("Please provide a proper body!");
            }

            existedTeam.Description = team.Description;
            existedTeam.Name = team.Name;
            
            var result = await teamRepository.Update(existedTeam);

            return Ok(result);

        }

        // Takıma kullanıcı ekleme
        [HttpPost("{teamId}/add-user")]
        public async Task<IActionResult> AddUserToTeam(int teamId, [FromBody] int userId)
        {
            var team = await teamRepository.GetById(teamId);
            var user = await userRepository.GetById(userId);

            if (team == null || user == null) return NotFound();

            var teamMember = new TeamMember { TeamId = teamId, UserId = userId };

            await teamMemberRepository.Create(teamMember);
            return Ok(teamMember);
        }

        // Takımdan kullanıcı çıkarma
        [HttpPost("{teamId}/remove-user")]
        public async Task<IActionResult> RemoveUserFromTeam(int teamId, [FromBody] int userId)
        {
            var teamMember = await teamMemberRepository.GetMemberShip(teamId, userId);
            if (teamMember == null) return NotFound();

            await teamMemberRepository.Delete(teamMember);

            return Ok();
        }
    }
}

