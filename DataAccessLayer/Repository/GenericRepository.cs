using DataAccessLayer.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext appDbContext;

        public GenericRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<T> Create(T entity)
        {
            await appDbContext.Set<T>().AddAsync(entity);
            await appDbContext.SaveChangesAsync();
            return entity;
        }

        public async Task Delete(T entity)
        {
            var result = appDbContext.Set<T>().Remove(entity);
            await appDbContext.SaveChangesAsync();
        }

        public async Task<List<T>> GetAll()
        {
            var result = await appDbContext.Set<T>().ToListAsync();
            return result;
        }

        public async Task<T> GetById(int Id)
        {
            var result = await appDbContext.Set<T>().FindAsync(Id);
            if(result == null)
            {
                throw new Exception("Not Found");
            }
            return result;
        }

        public async Task Update(T entity)
        {
            appDbContext.Set<T>().Update(entity);
            await appDbContext.SaveChangesAsync();
        }
    }
}
