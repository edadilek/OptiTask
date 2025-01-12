using DataAccessLayer.Entity;

namespace DataAccessLayer.Interface
{
    //generic altyapıyı kullanıp kod tekrarını engelliyoruz CRUD işlemleri için
    public interface IProjectTaskRepository : IGenericRepository<ProjectTask>
    {
        Task<ProjectTask> GetProjectTask(int projectId, int taskId);
    }
}
