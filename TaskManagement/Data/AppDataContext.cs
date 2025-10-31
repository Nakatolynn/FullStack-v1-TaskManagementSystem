using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.TaskManagement.Models;
using TaskManagementAPI.User;

namespace TaskManagementAPI.Data
{
    public class AppDataContext : DbContext
    {
        public AppDataContext(DbContextOptions<AppDataContext> options)
            : base(options)
        {
        }

        public DbSet<TasksCreateModel> Tasks { get; set; }
        public DbSet<Users> Users { get; set; }
        //   protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     base.OnModelCreating(modelBuilder);

        //     modelBuilder.Entity<TasksCreateModel>()
        //         .HasMany(t => t.SubTasks)
        //         .WithOne(t => t.ParentTask)
        //         .HasForeignKey(t => t.ParentTaskId)
        //         .OnDelete(DeleteBehavior.Cascade);
        // }
    }
}
