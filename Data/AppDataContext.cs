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

        public DbSet<TaskItemModel> TaskItems { get; set; }
        public DbSet<Users> Users {  get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    // Configure TaskItem table
        //    modelBuilder.Entity<TaskItemModel>()
        //        .ToTable("TaskItems");

        //    // Default value for CreatedAt
        //    modelBuilder.Entity<TaskItemModel>()
        //        .Property(t => t.CreatedAt)
        //        .HasDefaultValueSql("GETUTCDATE()");

        //    // Store enum as string (Pending, InProgress, etc.)
        //    modelBuilder.Entity<TaskItemModel>()
        //        .Property(t => t.Status)
        //        .HasConversion<string>()
        //        .HasMaxLength(20);
        //}
    }
}
