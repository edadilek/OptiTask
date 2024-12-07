using DataAccessLayer.Entity;
using DataAccessLayer.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class TeamMemberRepository : GenericRepository<TeamMember>, ITeamMemberRepository
    {
        private readonly AppDbContext _appDbContext;

        public TeamMemberRepository(AppDbContext appDbContext) : base(appDbContext)
        {

            _appDbContext = appDbContext;
        }

        public async Task<TeamMember> GetMemberShip(int teamId, int userId)
        {
            var membership = await _appDbContext.TeamMembers.FindAsync(teamId, userId);

            return membership;
        }
    }
}
