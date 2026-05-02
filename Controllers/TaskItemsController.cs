using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskTracker.API.Data;
using TaskTracker.API.Models;

namespace TaskTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskItemsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public TaskItemsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<TaskItem>>> GetAllTasks()
        {
            var tasks = await _context.TaskItems.ToListAsync();
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetTaskById(int id)
        {
            var task = await _context.TaskItems.FindAsync(id);
            if (task is null)
            {
                return NotFound($"Task with ID {id} was not found");
            }
            return Ok(task);
        }
        [HttpPost]
        public async Task<ActionResult> CreateTask(TaskItem task)
        {
            task.Id = 0;
            _context.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTask(int id, TaskItem taskItem)
        {
            if (id != taskItem.Id)
                return BadRequest($"route id {id} and body id {taskItem.Id} does not match");

            var task = await _context.TaskItems.FindAsync(id);

            if (task is null)
                return NotFound($"task with id {id} could not be found");

            task.Title = taskItem.Title;
            task.Description = taskItem.Description;
            task.DueDate = taskItem.DueDate;
            task.IsCompleted = taskItem.IsCompleted;
            task.Priority = taskItem.Priority;

            await _context.SaveChangesAsync();
            return Ok(task);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTask(int id)
        {
            var task = await _context.TaskItems.FindAsync(id);

            if (task is null)
                return NotFound($"task with id {id} could not be found");

            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
