using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.TaskManagement.Models;
using TaskManagementAPI.Data.Repositories;
namespace TaskManagementAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
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
            var tasks = await _taskService.GetAllAsync();
            return Ok(tasks);
        }
        [HttpGet("get-task-by-id")]
        public async Task<IActionResult> GetById(string id)
        {
            if (!Guid.TryParse(id, out Guid taskId))
                return BadRequest("Invalid GUID format for id.");

            var task = await _taskService.GetByIdAsync(taskId);
            if (task == null)
                return NotFound();

            return Ok(task);
        }


        [HttpPost("create-task")]
        public async Task<IActionResult> CreateTask([FromBody] TasksCreateModel model)
        {
            model.TaskId = Guid.NewGuid();
            var createdTask = await _taskService.CreateAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = createdTask.TaskId }, createdTask);
        }

        [HttpPut("update-task")]
        public async Task<IActionResult> UpdateTask(UpdateTaskModel model)
        {

            var updatedTask = await _taskService.UpdateAsync(model);
            if (updatedTask == null)
                return NotFound();

            return Ok(updatedTask);
        }

        [HttpDelete("delete-task/{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var deleted = await _taskService.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }

}
