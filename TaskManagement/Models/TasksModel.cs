using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using TaskManagementAPI.TaskManagement.Models;
using System.ComponentModel;
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

        [MaxLength(100)]
        public string? UserId { get; set; }
        public string? CreatedByUser { get; set; }

        public DateTime? DueDate { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public DateTime? ReviewDate { get; set; }
        public bool IsComplete { get; set; } = false;
        public DateTime? CompletionDate { get; set; }

        public Guid? ParentTaskId { get; set; }

        // [ForeignKey("ParentTaskId")]
        // public TasksCreateModel? ParentTask { get; set; }
        [NotMapped]
        public List<TaskViewModel>? SubTasks { get; set; }
    }


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
    public TaskStatus Status { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedByUserName { get; set; }

    public DateTime? DueDate { get; set; }
    public DateTime? SubmissionDate { get; set; }
    public string? ReviewRemarks { get; set; }
    public DateTime? ReviewDate { get; set; }
    public string? CompletionRemarks { get; set; }
    public bool? IsComplete { get; set; }
    public DateTime? CompletionDate { get; set; }
    public Guid? ParentTaskId { get; set; }
    public bool? IsDeleted { get; set; }
    public List<UpdateTaskModel>? SubTasks { get; set; }
    [MaxLength(100)]
    public string? UserId { get; set; }
}

[NotMapped]
public class TaskViewModel
{
    public Guid TaskId { get; set; }
    public string? TaskName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? SubmissionDate { get; set; }
    public DateTime? ReviewDate { get; set; }
    public string? ReviewRemarks { get; set; }
    public bool IsComplete { get; set; }
    public string? CompletionRemarks { get; set; }
    public DateTime? CompletionDate { get; set; }
    public Guid? ParentTaskId { get; set; }

    // public TasksCreateModel? ParentTask { get; set; }
    // public List<TaskViewModel>? SubTasks { get; set; }
    public TaskStatus TaskStatus { get; set; }
    public decimal? NumberOfSubTasks { get; set; }
    public bool? IsDeleted { get; set; }
    public TaskStatus Status { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UserId { get; set; }

}


public enum TaskStatus
{
    [Description("Not Done")]
    NotDone = 0,

    [Description("Pending")]
    Pending = 1,
    [Description("In Progress")]
    InProgress = 2,

    [Description("In Review")]
    InReview = 3,

    [Description("Completed")]
    Completed = 4,

    [Description("Closed")]
    Closed = 5,
}


