using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VLCTraders.Api.Data;
using VLCTraders.Api.Models.Common;

namespace VLCTraders.Api.Controllers
{
    [ApiController]
    [Route("api/v1/reports")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("monthly-sales")]
        public async Task<ActionResult<ApiResponse<object>>> MonthlySales([FromQuery] string month)
        {
            var result = await _context.Sales
                .Where(s => s.SaleDate.ToString("yyyy-MM") == month)
                .GroupBy(_ => 1)
                .Select(g => new { totalSales = g.Sum(s => s.BillAmount) })
                .FirstOrDefaultAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Monthly sales report retrieved successfully",
                Data = result ?? new { totalSales = 0m }
            });
        }

        [HttpGet("monthly-purchases")]
        public async Task<ActionResult<ApiResponse<object>>> MonthlyPurchases()
        {
            var total = await _context.Purchases.SumAsync(p => p.TotalCost);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Monthly purchases report retrieved successfully",
                Data = new { totalPurchases = total }
            });
        }

        [HttpGet("monthly-expenses")]
        public async Task<ActionResult<ApiResponse<object>>> MonthlyExpenses()
        {
            var total = await _context.Expenses.SumAsync(e => e.Amount);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Monthly expenses report retrieved successfully",
                Data = new { totalExpenses = total }
            });
        }

        [HttpGet("pnl")]
        public async Task<ActionResult<ApiResponse<object>>> PnlReport()
        {
            var totalSales = await _context.Sales.SumAsync(s => s.BillAmount);
            var totalExpenses = await _context.Expenses.SumAsync(e => e.Amount);
            var totalPurchases = await _context.Purchases.SumAsync(p => p.TotalCost);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "P&L report retrieved successfully",
                Data = new
                {
                    totalSales,
                    totalPurchases,
                    totalExpenses,
                    netProfit = totalSales - totalPurchases - totalExpenses
                }
            });
        }
    }
}
