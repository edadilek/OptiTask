using DataAccessLayer.Entity;
using DataAccessLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    //interface implementasyonlarını yapıyoruz
    public class ProjectRepository : GenericRepository<Project>, IProjectRepository
    {
        private readonly AppDbContext _appDbContext;
        public ProjectRepository(AppDbContext appDbContext) : base(appDbContext) 
        {

            _appDbContext = appDbContext;

        }
    }
}
