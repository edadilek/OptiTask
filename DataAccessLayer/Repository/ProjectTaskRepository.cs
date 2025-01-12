using DataAccessLayer.Entity;
using DataAccessLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    //interface implementasyonlarını yapıyoruz
    public class ProjectTaskRepository : GenericRepository<ProjectTask>, IProjectTaskRepository
    {
        private readonly AppDbContext _context;

        public ProjectTaskRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ProjectTask> GetProjectTask(int  projectId, int taskId)
        {
            var projectTask = await _context.Set<ProjectTask>().FindAsync(projectId, taskId);

            return projectTask;
        }
    }
}
