using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using TaskManagementAPI.TaskManagement.Models;

namespace TaskManagementAPI.Data.Repositories
{
    public interface ITaskManagementService
    {
        public Task<IEnumerable<TasksCreateModel>> GetAllAsync();
        public Task<TasksCreateModel?> GetByIdAsync(Guid id);
        public Task<TasksCreateModel> CreateAsync(TasksCreateModel task);
        public Task<TaskViewModel?> UpdateAsync(UpdateTaskModel task);
        public Task<bool> DeleteAsync(Guid id);
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

        public async Task<IEnumerable<TasksCreateModel>> GetAllAsync()
        {
            return await _context.Tasks
                .AsNoTracking()
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<TasksCreateModel?> GetByIdAsync(Guid id)
        {
            var action = "ITaskManagementService:GetTaskById";
            _logger.LogInformation($"{action}- Request Data: {JsonConvert.SerializeObject(id)}");
            try
            {
                return await _context.Tasks.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{action} - Unexpected Error: {ex.Message}", ex);
                throw new ApplicationException("An unexpected error occurred while retrieving tasks.", ex);
            }


        }

        public async Task<TasksCreateModel> CreateAsync(TasksCreateModel CreateTaskModel)
        {
            var action = "ITaskManagementService:CreatingTask";
            _logger.LogInformation($"{action}- Request Data: {JsonConvert.SerializeObject(CreateTaskModel)}");
            try
            {
                CreateTaskModel.CreatedAt = DateTime.UtcNow;
                _context.Tasks.Add(CreateTaskModel);
                await _context.SaveChangesAsync();
                return CreateTaskModel;
            }
            catch (Exception ex)
            {

                _logger.LogError($"{action} - Unexpected Error: {ex.Message}", ex);
                throw new ApplicationException("An unexpected error occurred while creating tasks.", ex);
            }

        }

        public async Task<TaskViewModel?> UpdateAsync(UpdateTaskModel UpdateTaskModel)
        {
            var action = "ITaskManagementService:UpdatingTask";
            _logger.LogInformation($"{action}- Request Data: {JsonConvert.SerializeObject(UpdateTaskModel)}");
            try
            {
                if (UpdateTaskModel == null)
                    throw new ArgumentNullException("Null update model not allowed");
                var originalTask = await _context.Tasks.FirstOrDefaultAsync(c => c.TaskId == UpdateTaskModel.TaskId);
                if (originalTask == null)
                    throw new ArgumentException($"CoI Activity with Id {UpdateTaskModel.TaskId} Not Found");
                originalTask.TaskName = UpdateTaskModel.TaskName;
                originalTask.Description = UpdateTaskModel.Description;
                originalTask.Status = UpdateTaskModel.Status;
                originalTask.UpdatedAt = DateTime.UtcNow;
                _context.Tasks.Update(originalTask);
                await _context.SaveChangesAsync();
                return new TaskViewModel
                {
                    TaskId = originalTask.TaskId,
                    TaskName = originalTask.TaskName,
                    Description = originalTask.Description,
                    Status = originalTask.Status,
                    CreatedAt = originalTask.CreatedAt,
                    UpdatedAt = originalTask.UpdatedAt
                };

            }
            catch (Exception ex)
            {
                _logger.LogError($"{action} - Unexpected Error: {ex.Message}", ex);
                throw new ApplicationException("An unexpected error occurred while updating tasks.", ex);
            }

        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var action = "ITaskManagementService:DeletingTask";
            _logger.LogInformation($"{action}- Request Data: {JsonConvert.SerializeObject(id)}");
            try
            {
                var task = await _context.Tasks.FindAsync(id);
                if (task == null)
                    return false;

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
    }
}
