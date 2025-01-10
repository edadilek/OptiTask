using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface ITeamProjectRepository : IGenericRepository<TeamProject>
    {
        Task<TeamProject> GetTeamProjectAsync(int teamId, int projectId);
    }
}
