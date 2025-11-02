using InterviewTest.Entities;
using Microsoft.EntityFrameworkCore;

namespace InterviewTest.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<User>().HasIndex(u => u.Email).IsUnique(); // helps de-dup
        }
    }
}
