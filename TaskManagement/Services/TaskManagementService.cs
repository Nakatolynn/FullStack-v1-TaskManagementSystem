using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TaskManagementAPI.TaskManagement.Models;

namespace TaskManagementAPI.Data.Repositories
{
    public interface ITaskManagementService
    {
        Task<IEnumerable<TasksCreateModel>> GetAllTasks();
        Task<IEnumerable<TasksCreateModel>> GetTaskByUserId(Guid userId);
        Task<TasksCreateModel?> GetTaskById(Guid id);
        Task<TasksCreateModel> CreateTask(TasksCreateModel createTaskModel);
        Task<TaskViewModel?> UpdateTask(UpdateTaskModel updateTaskModel);
        Task<bool> DeleteTask(Guid id);
        Task<(IEnumerable<TasksCreateModel> Tasks, int TotalCount)> ListOfPaginatedTasks(int pageNumber, int pageSize);
    }

    public class TaskManagementService : ITaskManagementService
    {
        private readonly AppDataContext _context;
        private readonly ILogger<TaskManagementService> _logger;

        public TaskManagementService(AppDataContext context, ILogger<TaskManagementService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<IEnumerable<TasksCreateModel>> GetAllTasks()
        {
            return await _context.Tasks
                .Where(t => t.ParentTaskId == null)
                .AsNoTracking()
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new TasksCreateModel
                {
                    TaskId = t.TaskId,
                    TaskName = t.TaskName,
                    Description = t.Description,
                    CreatedByUser = t.CreatedByUser,
                    CreatedAt = t.CreatedAt,
                    DueDate = t.DueDate,
                    Status = t.Status,
                    UserId = t.UserId,
                    SubTasks = _context.Tasks
                        .Where(st => st.ParentTaskId == t.TaskId)
                        .Select(st => new TaskViewModel
                        {
                            TaskId = st.TaskId,
                            TaskName = st.TaskName,
                            Description = st.Description,
                            CreatedByUserName = st.CreatedByUser,
                            CreatedAt = st.CreatedAt,
                            DueDate = st.DueDate,
                            Status = st.Status,
                            UserId = st.UserId
                        }).ToList()
                })
                .ToListAsync();
        }


        public async Task<TasksCreateModel?> GetTaskById(Guid id)
        {
            var action = "ITaskManagementService:GetTaskById";
            _logger.LogInformation($"{action}- Request Data: {JsonConvert.SerializeObject(id)}");

            try
            {
                var task = await _context.Tasks.FirstOrDefaultAsync(t => t.TaskId == id);
                if (task == null)
                    return null;

                task.SubTasks = await _context.Tasks
                    .Where(st => st.ParentTaskId == id)
                    .Select(st => new TaskViewModel
                    {
                        TaskId = st.TaskId,
                        TaskName = st.TaskName,
                        Description = st.Description,
                        CreatedByUserName = st.CreatedByUser,
                        CreatedAt = st.CreatedAt,
                        DueDate = st.DueDate,
                        Status = st.Status,
                        UserId = st.UserId
                    }).ToListAsync();

                return task;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{action} - Unexpected Error: {ex.Message}", ex);
                throw new ApplicationException("An unexpected error occurred while retrieving tasks.", ex);
            }
        }

        public async Task<IEnumerable<TasksCreateModel>> GetTaskByUserId(Guid userId)
        {
            var uid = userId.ToString();

            var userTasks = await _context.Tasks
                .AsNoTracking()
                .Where(t => t.UserId == uid && t.ParentTaskId == null)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            // Load sub-tasks for each
            foreach (var task in userTasks)
            {
                task.SubTasks = await _context.Tasks
                    .Where(st => st.ParentTaskId == task.TaskId)
                    .Select(st => new TaskViewModel
                    {
                        TaskId = st.TaskId,
                        TaskName = st.TaskName,
                        Description = st.Description,
                        CreatedByUserName = st.CreatedByUser,
                        CreatedAt = st.CreatedAt,
                        DueDate = st.DueDate,
                        Status = st.Status,
                        UserId = st.UserId
                    }).ToListAsync();
            }

            return userTasks;
        }
        public async Task<TasksCreateModel> CreateTask(TasksCreateModel createTaskModel)
        {
            var action = "ITaskManagementService:CreatingTask";
            _logger.LogInformation($"{action}- Request Data: {JsonConvert.SerializeObject(createTaskModel)}");

            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                if (createTaskModel.TaskId == Guid.Empty)
                    createTaskModel.TaskId = Guid.NewGuid();
                createTaskModel.CreatedAt = DateTime.UtcNow;
                createTaskModel.DueDate = createTaskModel.DueDate?.ToUniversalTime();
                createTaskModel.SubmissionDate = createTaskModel.SubmissionDate?.ToUniversalTime();
                createTaskModel.ReviewDate = createTaskModel.ReviewDate?.ToUniversalTime();
                createTaskModel.CompletionDate = createTaskModel.CompletionDate?.ToUniversalTime();

                List<TasksCreateModel> savedSubTasks = new List<TasksCreateModel>();

                if (createTaskModel.SubTasks != null && createTaskModel.SubTasks.Any())
                {
                    foreach (var subtask in createTaskModel.SubTasks)
                    {
                        var subTask = new TasksCreateModel
                        {
                            TaskId = Guid.NewGuid(),
                            TaskName = subtask.TaskName ?? "",
                            Description = subtask.Description,
                            CreatedByUser = subtask.CreatedByUserName,
                            UserId = subtask.UserId,
                            ParentTaskId = createTaskModel.TaskId,
                            Status = subtask.Status,
                            DueDate = subtask.DueDate?.ToUniversalTime(),
                            CreatedAt = DateTime.UtcNow,
                            SubmissionDate = subtask.SubmissionDate?.ToUniversalTime(),
                            ReviewDate = subtask.ReviewDate?.ToUniversalTime(),
                            CompletionDate = subtask.CompletionDate?.ToUniversalTime()
                        };

                        _context.Tasks.Add(subTask);
                        savedSubTasks.Add(subTask);
                    }
                }
                _context.Tasks.Add(createTaskModel);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                createTaskModel.SubTasks = savedSubTasks
                    .Select(s => new TaskViewModel
                    {
                        TaskId = s.TaskId,
                        TaskName = s.TaskName,
                        Description = s.Description,
                        UserId = s.UserId,
                        CreatedByUserName = s.CreatedByUser,
                        DueDate = s.DueDate,
                        ParentTaskId = s.ParentTaskId,
                        Status = s.Status,
                        CreatedAt = s.CreatedAt
                    }).ToList();

                return createTaskModel;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{action} - Unexpected Error: {ex.Message}", ex);
                await _context.Database.RollbackTransactionAsync();
                throw new ApplicationException("An unexpected error occurred while creating tasks.", ex);
            }
        }



        public async Task<TaskViewModel?> UpdateTask(UpdateTaskModel updateTaskModel)
        {
            var action = "ITaskManagementService:UpdatingTask";
            _logger.LogInformation($"{action}- Request Data: {JsonConvert.SerializeObject(updateTaskModel)}");

            try
            {
                var originalTask = await _context.Tasks.FirstOrDefaultAsync(c => c.TaskId == updateTaskModel.TaskId);
                if (originalTask == null)
                    throw new ArgumentException($"Task with Id {updateTaskModel.TaskId} not found");

                originalTask.TaskName = updateTaskModel.TaskName;
                originalTask.Description = updateTaskModel.Description;
                originalTask.Status = updateTaskModel.Status;
                originalTask.UpdatedAt = DateTime.UtcNow;
                originalTask.DueDate = EnsureUtc(updateTaskModel.DueDate);
                originalTask.SubmissionDate = EnsureUtc(updateTaskModel.SubmissionDate);
                originalTask.ReviewDate = EnsureUtc(updateTaskModel.ReviewDate);
                originalTask.CompletionDate = EnsureUtc(updateTaskModel.CompletionDate);


                if (updateTaskModel.ParentTaskId.HasValue)
                {
                    var parentExists = await _context.Tasks.AnyAsync(t => t.TaskId == updateTaskModel.ParentTaskId.Value);
                    if (!parentExists)
                        throw new ArgumentException($"Parent Task with Id {updateTaskModel.ParentTaskId} not found");
                    originalTask.ParentTaskId = updateTaskModel.ParentTaskId;
                }

                if (updateTaskModel.SubTasks != null && updateTaskModel.SubTasks.Any())
                {
                    foreach (var st in updateTaskModel.SubTasks)
                    {
                        var subTask = await _context.Tasks.FirstOrDefaultAsync(t => t.TaskId == st.TaskId);
                        if (subTask != null)
                        {
                            subTask.TaskName = st.TaskName;
                            subTask.Description = st.Description;
                            subTask.Status = st.Status;
                            subTask.UpdatedAt = DateTime.UtcNow;
                            subTask.ParentTaskId = originalTask.TaskId;

                            subTask.DueDate = EnsureUtc(st.DueDate);
                            subTask.SubmissionDate = EnsureUtc(st.SubmissionDate);
                            subTask.ReviewDate = EnsureUtc(st.ReviewDate);
                            subTask.CompletionDate = EnsureUtc(st.CompletionDate);
                        }
                        else
                        {
                            _context.Tasks.Add(new TasksCreateModel
                            {
                                TaskId = Guid.NewGuid(),
                                TaskName = st.TaskName,
                                Description = st.Description,
                                UserId = st.UserId,
                                CreatedByUser = st.CreatedByUserName,
                                Status = st.Status,
                                ParentTaskId = originalTask.TaskId,
                                CreatedAt = DateTime.UtcNow,
                                DueDate = EnsureUtc(st.DueDate),
                                SubmissionDate = EnsureUtc(st.SubmissionDate),
                                ReviewDate = EnsureUtc(st.ReviewDate),
                                CompletionDate = EnsureUtc(st.CompletionDate)
                            });
                        }
                    }
                }

                _context.Tasks.Update(originalTask);
                await _context.SaveChangesAsync();

                var subTasksView = await _context.Tasks
                    .Where(t => t.ParentTaskId == originalTask.TaskId)
                    .Select(st => new TaskViewModel
                    {
                        TaskId = st.TaskId,
                        TaskName = st.TaskName,
                        Description = st.Description,
                        Status = st.Status,
                        CreatedByUserName = st.CreatedByUser,
                        UserId = st.UserId,
                        CreatedAt = st.CreatedAt,
                        UpdatedAt = st.UpdatedAt,
                        DueDate = st.DueDate,
                        SubmissionDate = st.SubmissionDate,
                        ReviewDate = st.ReviewDate,
                        CompletionDate = st.CompletionDate,
                        IsComplete = st.IsComplete,
                        ParentTaskId = st.ParentTaskId
                    }).ToListAsync();

                return new TaskViewModel
                {
                    TaskId = originalTask.TaskId,
                    TaskName = originalTask.TaskName,
                    Description = originalTask.Description,
                    Status = originalTask.Status,
                    CreatedAt = originalTask.CreatedAt,
                    UpdatedAt = originalTask.UpdatedAt,
                    DueDate = originalTask.DueDate,
                    SubmissionDate = originalTask.SubmissionDate,
                    ReviewDate = originalTask.ReviewDate,
                    CompletionDate = originalTask.CompletionDate,
                    IsComplete = originalTask.IsComplete,
                    ParentTaskId = originalTask.ParentTaskId,
                    UserId = originalTask.UserId,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"{action} - Unexpected Error: {ex.Message}", ex);
                throw new ApplicationException("An unexpected error occurred while updating tasks.", ex);
            }
        }

        public async Task<(IEnumerable<TasksCreateModel> Tasks, int TotalCount)> ListOfPaginatedTasks(int pageNumber, int pageSize)
        {
            var totalCount = await _context.Tasks.CountAsync(t => t.ParentTaskId == null);

            var tasks = await _context.Tasks
                .Where(t => t.ParentTaskId == null)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var taskIds = tasks.Select(t => t.TaskId).ToList();

            var subTasks = await _context.Tasks
                .Where(st => st.ParentTaskId != null && taskIds.Contains(st.ParentTaskId.Value))
                .ToListAsync();

            foreach (var task in tasks)
            {
                task.SubTasks = subTasks
                    .Where(st => st.ParentTaskId == task.TaskId)
                    .Select(st => new TaskViewModel
                    {
                        TaskId = st.TaskId,
                        TaskName = st.TaskName,
                        Description = st.Description,
                        CreatedByUserName = st.CreatedByUser,
                        CreatedAt = st.CreatedAt,
                        DueDate = st.DueDate,
                        Status = st.Status,
                        UserId = st.UserId,
                        ParentTaskId = st.ParentTaskId
                    }).ToList();
            }

            return (tasks, totalCount);
        }



        public async Task<bool> DeleteTask(Guid id)
        {
            var action = "ITaskManagementService:DeletingTask";
            _logger.LogInformation($"{action}- Request Data: {JsonConvert.SerializeObject(id)}");

            try
            {
                var task = await _context.Tasks.FindAsync(id);
                if (task == null)
                    return false;
                var subTasks = _context.Tasks.Where(st => st.ParentTaskId == id);
                _context.Tasks.RemoveRange(subTasks);

                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{action} - Unexpected Error: {ex.Message}", ex);
                throw new ApplicationException("An unexpected error occurred while deleting tasks.", ex);
            }
        }

        private static DateTime? EnsureUtc(DateTime? date)
        {
            if (date == null) return null;

            if (date.Value.Kind == DateTimeKind.Utc)
                return date;

            return DateTime.SpecifyKind(date.Value, DateTimeKind.Utc);
        }


    }
}
