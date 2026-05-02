using Microsoft.EntityFrameworkCore;
using TaskTracker.API.Models;

namespace TaskTracker.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        public DbSet<TaskItem>  TaskItems { get; set; }
    }
}
