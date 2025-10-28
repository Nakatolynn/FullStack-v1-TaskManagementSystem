using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
namespace TaskManagementAPI.TaskManagement.Models
{
    [Table("Tasks")]
    public class TasksCreateModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TaskId { get; internal set; } = Guid.NewGuid();

        [Required]
        [MaxLength(150)]
        public string TaskName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public TaskStatus Status { get; set; } = TaskStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
        
        // stored as text in DB (migration created UserId as text)
        [MaxLength(100)]
        public string? UserId { get; set; }

    }


    public enum TaskStatus
    {
        Pending,
        InProgress,
        Completed,
        Closed,
    }
    public class UpdateTaskModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid TaskId { get; set; }

        [Required]
        [MaxLength(150)]
        public string TaskName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public TaskStatus Status { get; set; } = TaskStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        public string? UserId { get; set; }
    }
    public class TaskViewModel
    {
        public Guid TaskId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UserId { get; set; }
    }

}





