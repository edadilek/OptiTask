using DataAccessLayer.Entity;

namespace DataAccessLayer.Interface
{
    public interface IProjectTaskRepository : IGenericRepository<ProjectTask>
    {
        Task<ProjectTask> GetProjectTask(int projectId, int taskId);
    }
}
