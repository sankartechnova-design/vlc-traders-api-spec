using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VLCTraders.Api.Data;
using VLCTraders.Api.Models.Common;
using VLCTraders.Api.Models.Domain;

namespace VLCTraders.Api.Controllers
{
    [ApiController]
    [Route("api/v1/crm")]
    [Authorize]
    public class CrmController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CrmController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("visits")]
        public async Task<ActionResult<ApiResponse<CrmVisit>>> AddCustomerVisit([FromBody] CrmVisit request)
        {
            var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == request.CustomerId);
            if (!customerExists)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Customer not found"
                });
            }

            if (request.NextFollowupDate.HasValue && request.NextFollowupDate.Value < request.VisitDate)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Follow-up date cannot be before visit date"
                });
            }

            request.CreatedBy = User.Identity?.Name ?? "admin";
            _context.CrmVisits.Add(request);
            await _context.SaveChangesAsync();

            return StatusCode(201, new ApiResponse<CrmVisit>
            {
                Success = true,
                Message = "Visit added successfully",
                Data = request
            });
        }

        [HttpGet("visits")]
        public async Task<ActionResult<ApiResponse<List<CrmVisit>>>> GetVisitHistory()
        {
            var visits = await _context.CrmVisits
                .Include(v => v.Customer)
                .OrderByDescending(v => v.VisitDate)
                .ToListAsync();

            return Ok(new ApiResponse<List<CrmVisit>>
            {
                Success = true,
                Message = "Visit history retrieved successfully",
                Data = visits
            });
        }

        [HttpGet("followups/due")]
        public async Task<ActionResult<ApiResponse<List<object>>>> GetDueFollowups()
        {
            var today = DateTime.UtcNow.Date;
            var followups = await _context.CrmVisits
                .Where(v => v.NextFollowupDate.HasValue && v.NextFollowupDate.Value.Date <= today)
                .Include(v => v.Customer)
                .Select(v => new
                {
                    CustomerName = v.Customer.CustomerName,
                    NextFollowup = v.NextFollowupDate.Value
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<object>>
            {
                Success = true,
                Message = "Due followups retrieved successfully",
                Data = followups.Cast<object>().ToList()
            });
        }
    }
}
