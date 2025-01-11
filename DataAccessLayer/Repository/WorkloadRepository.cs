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
    public class WorkloadRepository : GenericRepository<Workload>, IWorkloadRepository
    {
        private readonly AppDbContext _context;

        public WorkloadRepository(AppDbContext context) : base(context) 
        {
            _context = context;
        }

        public new async Task<Workload> GetById(int id) // new anahtar kelimesi üst classtaki fonksiyonu gizler (override eder)
        {
            var workload = await _context.Set<Workload>().Include(w => w.user).FirstAsync(w => w.Id == id);
            return workload;
        }
    }
}
