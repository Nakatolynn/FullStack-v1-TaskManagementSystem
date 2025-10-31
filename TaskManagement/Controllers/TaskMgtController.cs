using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.TaskManagement.Models;
using TaskManagementAPI.Data.Repositories;
namespace TaskManagementAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class TaskManagementController : ControllerBase
    {
        private readonly ITaskManagementService _taskService;

        public TaskManagementController(ITaskManagementService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet("list-of-tasks")]
        public async Task<IActionResult> GetAll()
        {
            var tasks = await _taskService.GetAllTasks();
            return Ok(tasks);
        }
        [HttpGet("list-of-paginated-tasks")]
        public async Task<IActionResult> GetPaginatedTasks([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var pagedTasks = await _taskService.ListOfPaginatedTasks(page, pageSize);
            return Ok(pagedTasks);
        }

        [HttpGet("get-task-by-id")]
        public async Task<IActionResult> GetById(string id)
        {
            if (!Guid.TryParse(id, out Guid taskId))
                return BadRequest("Invalid GUID format for id.");

            var task = await _taskService.GetTaskById(taskId);
            if (task == null)
                return NotFound();

            return Ok(task);
        }

        [HttpGet("get-list-of-users-task-by-userId/{userId}")]
        public async Task<IActionResult> GetTasksByUser(string userId)
        {
            if (!Guid.TryParse(userId, out Guid uid))
                return BadRequest("Invalid GUID format for userId.");

            var tasks = await _taskService.GetTaskByUserId(uid);
            return Ok(tasks);
        }


        [HttpPost("create-task")]
        public async Task<IActionResult> CreateTask([FromBody] TasksCreateModel model)
        {
            model.TaskId = Guid.NewGuid();
            var createdTask = await _taskService.CreateTask(model);
            return CreatedAtAction(nameof(GetById), new { id = createdTask.TaskId }, createdTask);
        }

        [HttpPut("update-task")]
        public async Task<IActionResult> UpdateTask(UpdateTaskModel model)
        {

            var updatedTask = await _taskService.UpdateTask(model);
            if (updatedTask == null)
                return NotFound();

            return Ok(updatedTask);
        }

        [HttpDelete("delete-task/{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var deleted = await _taskService.DeleteTask(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }

}
