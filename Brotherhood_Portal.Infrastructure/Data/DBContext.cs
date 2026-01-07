using Brotherhood_Portal.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Brotherhood_Portal.Infrastructure.Context
{
    public class AppDBContext(DbContextOptions<AppDBContext> options) : DbContext(options)
    {
        /*DbSets*/
        public DbSet<AppUser> Users { get; set; }
    }
}
