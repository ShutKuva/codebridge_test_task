using Core.Db;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class DogsContext : DbContext
    {
        public DbSet<Dog> Dogs { get; set; }

        public DogsContext(DbContextOptions<DogsContext> options) : base(options)
        {

        }
    }
}