using Effective_Mobile.Models;
using Microsoft.EntityFrameworkCore;

namespace Effective_Mobile.Data
{
    public class ApplicationContext(DbContextOptions<ApplicationContext> options) : DbContext(options)
    {
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().Property(o => o.Id).ValueGeneratedOnAdd();
        }
    }
}
