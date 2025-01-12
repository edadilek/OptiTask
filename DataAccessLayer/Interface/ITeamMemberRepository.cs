using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    //generic altyapıyı kullanıp kod tekrarını engelliyoruz CRUD işlemleri için
    public interface ITeamMemberRepository : IGenericRepository<TeamMember>
    {
        Task<List<TeamMember>> GetTeamMembers(int teamId);
        Task<TeamMember> GetMemberShip(int teamId, int userId);
        Task<TeamMember> GetTeamByUserId(int userId);
    }
}
