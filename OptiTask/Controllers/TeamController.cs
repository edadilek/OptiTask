using Microsoft.AspNetCore.Mvc;
using DataAccessLayer.Entity;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer;
using DataAccessLayer.Repository;
using OptiTask.DTOs;
using DataAccessLayer.Interface;

namespace OptiTask.Controllers
{
    //team için endpoint işlemleri

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

        //Takımları listeleme
        [HttpGet]
        public async Task<IActionResult> GetAllTeams()
        {
            var teams = await teamRepository.GetAll();
            return Ok(teams);
        }

        //belli idye sahip takımı getirme
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeamById(int id)
        {
            var team = await teamRepository.GetById(id);

            if (team == null)
            {
                return NotFound();
            }

            return Ok(team);
        }

        //Yeni takım oluşturma
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

        //takımı silme
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            var team = await teamRepository.GetById(id);

            if (team == null)
            {
                return NotFound();
            }
        
            await teamRepository.Delete(team);

            return Ok(team);
        }

        //takımı güncelle
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeam(int id, [FromBody] TeamDTO team)
        {
            var existedTeam = await teamRepository.GetById(id);

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
        [HttpPost("{id}/add-user")]
        public async Task<IActionResult> AddUserToTeam(int id, [FromBody] int userId)
        {
            var user = await userRepository.GetById(userId);

            var teamMemberExist = await teamMemberRepository.GetMemberShip(id, userId);

            if( teamMemberExist != null)
            {
                return BadRequest("User already has a team!");
            }


            var team = await teamRepository.GetById(id);

            if (team == null || user == null) return NotFound();

            var teamMember = new TeamMember { TeamId = id, UserId = userId };

            await teamMemberRepository.Create(teamMember);
            return Ok(teamMember);
        }

        // Takımdan kullanıcı çıkarma
        [HttpPost("{id}/remove-user")]
        public async Task<IActionResult> RemoveUserFromTeam(int id, [FromBody] int userId)
        {
            var teamMember = await teamMemberRepository.GetMemberShip(id, userId);
            if (teamMember == null) return NotFound();

            await teamMemberRepository.Delete(teamMember);

            return Ok();
        }
    }
}

