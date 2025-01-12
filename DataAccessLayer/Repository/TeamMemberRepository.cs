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
    //interface implementasyonlarını yapıyoruz
    public class TeamMemberRepository : GenericRepository<TeamMember>, ITeamMemberRepository
    {
        private readonly AppDbContext _appDbContext;

        public TeamMemberRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<TeamMember>> GetTeamMembers(int teamId)
        {
            return await _appDbContext.TeamMembers
                .Include(tm => tm.User)  // User bilgilerini de yükle
                .Where(tm => tm.TeamId == teamId)
                .ToListAsync();
        }

        public async Task<TeamMember> GetMemberShip(int teamId, int userId)
        {
            var membership = await _appDbContext.TeamMembers
                .Include(tm => tm.User)  // User bilgilerini de yükle
                .FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.UserId == userId);
            return membership;
        }

        public async Task<TeamMember> GetTeamByUserId(int userId)
        {
            var membership = await _appDbContext.TeamMembers.FirstOrDefaultAsync(tm => tm.UserId == userId);

            return membership;
        }
    }
}
