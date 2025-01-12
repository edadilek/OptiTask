using DataAccessLayer.Entity;
using DataAccessLayer.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    //interface implementasyonlarını yapıyoruz
    public class WorkloadRepository : GenericRepository<Workload>, IWorkloadRepository
    {
        private readonly AppDbContext _context;

        public WorkloadRepository(AppDbContext context) : base(context) 
        {
            _context = context;
        }

        public async Task<Workload> GetByUserId(int userId)
        {
            var workload = await _context.Set<Workload>().Where(w => w.UserId == userId).FirstOrDefaultAsync();

            return workload;
        }
    }
}
