using DataAccessLayer.Entity;
using DataAccessLayer.Interface;

namespace DataAccessLayer.Repository
{
    //interface implementasyonlarını yapıyoruz
    public class TaskAssignmentRepository : GenericRepository<TaskAssignment>, ITaskAssignmentRepository
    {
        private readonly AppDbContext _appDbContext;
        public TaskAssignmentRepository(AppDbContext appDbContext) : base(appDbContext) 
        {

            _appDbContext = appDbContext;

        }

        public async Task<TaskAssignment> GetTaskAssignment(int taskId, int userId)
        {
            var taskAssignment = await _appDbContext.Set<TaskAssignment>().FindAsync(taskId, userId);

            return taskAssignment;
        }
    }
}
