using DataAccessLayer.Entity;
using DataAccessLayer.Interface;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository
{
    public class TeamProjectRepository : GenericRepository<TeamProject>, ITeamProjectRepository
    {
        private readonly AppDbContext _context;

        public TeamProjectRepository(AppDbContext context) : base(context) 
        {
            _context = context;
        }

        public async Task<TeamProject> GetTeamProjectAsync(int teamId, int projectId)
        {
            var teamProject = await _context.Set<TeamProject>().Where(el => el.teamId == teamId).Where(el => el.projectId == projectId).FirstOrDefaultAsync();

            if (teamProject == null) {
                return new TeamProject();
            }

            return teamProject;
        }
    }
}
