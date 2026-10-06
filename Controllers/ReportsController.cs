using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VLCTraders.Api.Data;
using VLCTraders.Api.Models.Common;

namespace VLCTraders.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NotificationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("pending-payments")]
        public async Task<ActionResult<ApiResponse<object>>> PendingPayments()
        {
            var pending = await _context.Sales
                .Where(s => s.PaymentStatus != "Completed")
                .Select(s => new { s.SaleId, s.InvoiceNo, s.BillAmount, s.PaymentStatus })
                .ToListAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Pending payments retrieved successfully",
                Data = pending
            });
        }

        [HttpGet("followups")]
        public async Task<ActionResult<ApiResponse<object>>> Followups()
        {
            var due = await _context.CrmVisits
                .Where(v => v.NextFollowupDate.HasValue)
                .Select(v => new { v.CustomerId, v.NextFollowupDate })
                .ToListAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Due followups retrieved successfully",
                Data = due
            });
        }

        [HttpGet("tasks")]
        public async Task<ActionResult<ApiResponse<object>>> Tasks()
        {
            var tasks = await _context.Tasks
                .Where(t => t.Status != "Completed")
                .Select(t => new { t.TaskId, t.TaskName, t.AssignedTo, t.ReminderDate })
                .ToListAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Tasks retrieved successfully",
                Data = tasks
            });
        }
    }
}
