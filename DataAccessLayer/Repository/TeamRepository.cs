using DataAccessLayer.Entity;
using DataAccessLayer.Interface;

namespace DataAccessLayer.Repository
{
    public class TeamRepository : GenericRepository<Team>, ITeamRepository
    {
        private readonly AppDbContext _appDbContext;
        public TeamRepository(AppDbContext appDbContext) : base(appDbContext) 
        {

            _appDbContext = appDbContext;

        }
    }
}
