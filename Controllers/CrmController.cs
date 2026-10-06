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
    public class ExpensesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExpensesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Expense>>>> GetExpenses()
        {
            var expenses = await _context.Expenses
                .OrderByDescending(e => e.ExpenseDate)
                .ToListAsync();

            return Ok(new ApiResponse<List<Expense>>
            {
                Success = true,
                Message = "Expenses retrieved successfully",
                Data = expenses
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Expense>>> CreateExpense([FromBody] Expense request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<Expense>
                {
                    Success = false,
                    Message = "Validation failed"
                });
            }

            var validTypes = new[] { "CE", "MVE", "MFE", "SE" };
            if (!validTypes.Contains(request.ExpenseType))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid expense type",
                    Errors =
                    [
                        new ApiError { Field = "expenseType", Message = "Allowed values: CE, MVE, MFE, SE" }
                    ]
                });
            }

            request.CreatedBy = User.Identity?.Name ?? "admin";
            _context.Expenses.Add(request);
            await _context.SaveChangesAsync();

            return StatusCode(201, new ApiResponse<Expense>
            {
                Success = true,
                Message = "Expense created successfully",
                Data = request
            });
        }
    }
}
