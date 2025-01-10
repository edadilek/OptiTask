namespace DataAccessLayer.Interface
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> Create (T entity);
        Task Delete (T entity);
        Task<T> Update(T entity);

        Task<List<T>> GetAll ();
        Task<T> GetById (int Id);

    }
}
