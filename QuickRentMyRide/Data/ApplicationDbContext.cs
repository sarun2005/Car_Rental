using Microsoft.EntityFrameworkCore;
using QuickRentMyRide.Models;

namespace QuickRentMyRide.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
    }
}
