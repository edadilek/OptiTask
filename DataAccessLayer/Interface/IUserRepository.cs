using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface IUserRepository :IGenericRepository<User>
    {
        Task<User> GetByEmail(string email);
        Task<bool> CheckIfUserExists(string email);
        Task<bool> CheckIfUserExists(int id);
    }
}
