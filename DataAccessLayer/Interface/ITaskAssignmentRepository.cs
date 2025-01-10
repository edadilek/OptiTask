using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface ITaskAssignmentRepository : IGenericRepository<TaskAssignment>
    {
        Task<TaskAssignment> GetTaskAssignment(int taskId, int userId);
    }
}
