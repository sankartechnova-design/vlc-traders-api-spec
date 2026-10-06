using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VLCTraders.Api.Data;
using VLCTraders.Api.Models.Common;
using VLCTraders.Api.Models.Domain;

namespace VLCTraders.Api.Controllers
{
    [ApiController]
    [Route("api/v1/bank")]
    [Authorize]
    public class BankController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BankController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("capital")]
        public async Task<ActionResult<ApiResponse<BankLedger>>> CapitalInjection([FromBody] BankLedger request)
        {
            if (request.Amount <= 0)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Amount must be greater than zero"
                });
            }

            var currentBalance = await _context.BankLedger
                .SumAsync(b => b.Amount);

            request.TransactionType = "Capital";
            request.BalanceBefore = currentBalance;
            request.BalanceAfter = currentBalance + request.Amount;
            request.CreatedBy = User.Identity?.Name ?? "admin";

            _context.BankLedger.Add(request);
            await _context.SaveChangesAsync();

            return StatusCode(201, new ApiResponse<BankLedger>
            {
                Success = true,
                Message = "Capital injected successfully",
                Data = request
            });
        }

        [HttpGet("ledger")]
        public async Task<ActionResult<ApiResponse<List<BankLedger>>>> GetBankLedger()
        {
            var ledger = await _context.BankLedger
                .OrderByDescending(l => l.TransactionDate)
                .ToListAsync();

            return Ok(new ApiResponse<List<BankLedger>>
            {
                Success = true,
                Message = "Bank ledger retrieved successfully",
                Data = ledger
            });
        }

        [HttpGet("current-balance")]
        public async Task<ActionResult<ApiResponse<object>>> GetCurrentBalance()
        {
            var currentBalance = await _context.BankLedger
                .SumAsync(b => b.Amount);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Current balance retrieved successfully",
                Data = new { currentBalance }
            });
        }
    }
}
