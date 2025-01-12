using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    //generic altyapıyı kullanıp kod tekrarını engelliyoruz CRUD işlemleri için
    public interface IWorkloadRepository : IGenericRepository<Workload>
    {
        Task<Workload> GetByUserId(int userId);
    }
}
