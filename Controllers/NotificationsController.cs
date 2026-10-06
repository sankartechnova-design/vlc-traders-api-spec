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
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<ApiResponse<object>>> GetDashboardSummary()
        {
            var totalSales = await _context.Sales.SumAsync(s => s.BillAmount);
            var totalExpenses = await _context.Expenses.SumAsync(e => e.Amount);
            var currentBalance = await _context.BankLedger.SumAsync(b => b.Amount);

            var result = new
            {
                totalSales,
                netProfit = totalSales - totalExpenses,
                grossProfit = await _context.Sales.SumAsync(s => s.GrossProfit),
                capitalInvested = await _context.BankLedger.Where(b => b.TransactionType == "Capital").SumAsync(b => b.Amount),
                currentBalance
            };

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Dashboard summary retrieved successfully",
                Data = result
            });
        }
    }
}
