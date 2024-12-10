using DataAccessLayer.Entity;
using DataAccessLayer.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly AppDbContext _appDbContext;
        public UserRepository(AppDbContext appDbContext) : base(appDbContext)
        {

            _appDbContext = appDbContext;
        }

        public async Task<User> GetByEmail(string email)
        {
            var result = await _appDbContext.Set<User>().Where(el => el.Mail == email).FirstOrDefaultAsync();

            return result;
        }
    }
}
