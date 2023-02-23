using Microsoft.EntityFrameworkCore;

namespace Net6WebApp.Entities
{
    public class DataBaseContext : DbContext
    {
        public DataBaseContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        
    }
   
}
