using DataAccessLayer.Entity;

namespace DataAccessLayer.Interface
{
    //generic altyapıyı kullanıp kod tekrarını engelliyoruz CRUD işlemleri için
    public interface IProjectRepository : IGenericRepository<Project>
    {

    }
}
