using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> Create (T entity);
        Task Delete (T entity);
        Task Update(T entity);

        Task<List<T>> GetAll ();
        Task<T> GetById (int Id);

    }
}
