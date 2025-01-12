namespace DataAccessLayer.Interface
{
    //generic repository oluşturuyoruz diğer repositoryler de buradakilere sahip olmuş olacak 
    public interface IGenericRepository<T> where T : class
    {
        Task<T> Create (T entity);
        Task Delete (T entity);
        Task<T> Update(T entity);

        Task<List<T>> GetAll ();
        Task<T> GetById (int Id);

    }
}
