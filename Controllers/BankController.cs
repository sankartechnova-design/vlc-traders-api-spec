using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VLCTraders.Api.Data;
using VLCTraders.Api.Models.Common;
using VLCTraders.Api.Models.Domain;

namespace VLCTraders.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<UserTask>>>> GetTasks()
        {
            var tasks = await _context.Tasks.OrderBy(t => t.TaskDate).ToListAsync();
            return Ok(new ApiResponse<List<UserTask>>
            {
                Success = true,
                Message = "Tasks retrieved successfully",
                Data = tasks
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<UserTask>>> CreateTask([FromBody] UserTask request)
        {
            if (request.ReminderDate.HasValue && request.ReminderDate.Value < request.TaskDate)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Reminder date must be greater than or equal to task date"
                });
            }

            request.CreatedBy = User.Identity?.Name ?? "admin";
            _context.Tasks.Add(request);
            await _context.SaveChangesAsync();

            return StatusCode(201, new ApiResponse<UserTask>
            {
                Success = true,
                Message = "Task created successfully",
                Data = request
            });
        }

        [HttpGet("due")]
        public async Task<ActionResult<ApiResponse<List<UserTask>>>> GetDueTasks()
        {
            var today = DateTime.UtcNow.Date;
            var tasks = await _context.Tasks
                .Where(t => t.ReminderDate.HasValue && t.ReminderDate.Value.Date <= today && t.Status != "Completed")
                .ToListAsync();

            return Ok(new ApiResponse<List<UserTask>>
            {
                Success = true,
                Message = "Due tasks retrieved successfully",
                Data = tasks
            });
        }
    }
}
