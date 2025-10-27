using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
namespace TaskManagementAPI.TaskManagement.Models
{
    public class TaskItemModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaskId { get; set; }

        [Required]
        [MaxLength(150)]
        public string TaskName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public TaskStatus Status { get; set; } = TaskStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
        public string? UserId { get; set; }

    }
    public enum TaskStatus
    {
        Pending,
        InProgress,
        Completed,
        Closed,
    }
}




