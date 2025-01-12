using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    //generic altyapıyı kullanıp kod tekrarını engelliyoruz CRUD işlemleri için
    public interface IUserRepository :IGenericRepository<User>
    {
        //kullanıcılar aynı emaile sahip olamayacağı için email üzerinden yapıyoruz
        Task<User> GetByEmail(string email);
        Task<bool> CheckIfUserExists(string email);
        Task<bool> CheckIfUserExists(int id);
    }
}
