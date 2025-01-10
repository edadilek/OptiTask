using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface ITeamMemberRepository : IGenericRepository<TeamMember>
    {
        Task<List<TeamMember>> GetTeamMembers(int teamId);
        Task<TeamMember> GetMemberShip(int teamId, int userId);
    }
}
