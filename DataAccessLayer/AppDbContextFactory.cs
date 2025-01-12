using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    //Migration işlemleri için gerekli bağlantı bilgilerini oluşturuyoruz
    //DbContext oluşturuyoruz
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql("Host=172.17.0.2;Port=5432;Database=optiTaskDb;Username=admin;Password=postgres;");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
