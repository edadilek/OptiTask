using DataAccessLayer.Entity;
using DataAccessLayer.Interface;

namespace DataAccessLayer.Repository
{
    public class TasksRepository : GenericRepository<Tasks>, ITasksRepository
    {
        private readonly AppDbContext _appDbContext;
        public TasksRepository(AppDbContext appDbContext) : base(appDbContext) 
        {

            _appDbContext = appDbContext;

        }

    }
}
