using DataAccessLayer.Entity;
using DataAccessLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class WorkloadRepository : GenericRepository<Workload>, IWorkloadRepository
    {
        private readonly AppDbContext _context;

        public WorkloadRepository(AppDbContext context) : base(context) 
        {
            _context = context;
        }
    }
}
