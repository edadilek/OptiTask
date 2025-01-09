using DataAccessLayer.Entity;
using DataAccessLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class TeamProjectRepository : GenericRepository<TeamProject>, ITeamProjectRepository
    {
        private readonly AppDbContext _context;

        public TeamProjectRepository(AppDbContext context) : base(context) 
        {
            _context = context;
        }
    }
}
