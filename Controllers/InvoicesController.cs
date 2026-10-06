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
    public class QuotationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public QuotationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Quotation>>>> GetQuotations()
        {
            var quotations = await _context.Quotations
                .Include(q => q.Customer)
                .Include(q => q.QuotationItems)
                .ThenInclude(qi => qi.Material)
                .OrderByDescending(q => q.QuotationDate)
                .ToListAsync();

            return Ok(new ApiResponse<List<Quotation>>
            {
                Success = true,
                Message = "Quotations retrieved successfully",
                Data = quotations
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Quotation>>> CreateQuotation([FromBody] Quotation request)
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

            if (request.ValidTill <= request.QuotationDate)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Valid till date must be greater than quotation date"
                });
            }

            if (request.QuotationItems == null || !request.QuotationItems.Any())
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Quotation must contain at least one item"
                });
            }

            request.TotalAmount = request.QuotationItems.Sum(i => i.LineTotal);
            request.CreatedBy = User.Identity?.Name ?? "admin";
            _context.Quotations.Add(request);
            await _context.SaveChangesAsync();

            return StatusCode(201, new ApiResponse<Quotation>
            {
                Success = true,
                Message = "Quotation created successfully",
                Data = request
            });
        }

        [HttpGet("{quotationId:int}/pdf")]
        public async Task<ActionResult> GetQuotationPdf(int quotationId)
        {
            var quotation = await _context.Quotations.FindAsync(quotationId);
            if (quotation == null)
            {
                return NotFound();
            }

            return File(System.Text.Encoding.UTF8.GetBytes($"Quotation PDF for {quotation.QuotationNo}"), "application/pdf", $"quotation-{quotationId}.pdf");
        }
    }
}
