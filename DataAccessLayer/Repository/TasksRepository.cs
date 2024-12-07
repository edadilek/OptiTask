using DataAccessLayer.Entity;
using DataAccessLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
