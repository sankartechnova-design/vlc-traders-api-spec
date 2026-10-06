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
    public class InvoicesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InvoicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Invoice>>> GenerateInvoice([FromBody] Invoice request)
        {
            var saleExists = await _context.Sales.AnyAsync(s => s.SaleId == request.SaleId && s.Status != "Cancelled");
            if (!saleExists)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Sale not found"
                });
            }

            request.InvoiceNumber = $"INV-{request.SaleId:0000}";
            request.InvoiceDate = DateTime.UtcNow;
            request.CreatedBy = User.Identity?.Name ?? "admin";

            _context.Invoices.Add(request);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<Invoice>
            {
                Success = true,
                Message = "Invoice generated successfully",
                Data = request
            });
        }

        [HttpGet("{invoiceId:int}/pdf")]
        public async Task<ActionResult> GetInvoicePdf(int invoiceId)
        {
            var invoice = await _context.Invoices.FindAsync(invoiceId);
            if (invoice == null)
            {
                return NotFound();
            }

            return File(System.Text.Encoding.UTF8.GetBytes($"Invoice PDF for {invoice.InvoiceNumber}"), "application/pdf", $"invoice-{invoiceId}.pdf");
        }

        [HttpGet("customer/{customerId:int}/statement")]
        public async Task<ActionResult<ApiResponse<List<object>>>> GetCustomerStatement(int customerId)
        {
            var sales = await _context.Sales
                .Where(s => s.CustomerId == customerId)
                .Select(s => new
                {
                    s.InvoiceNo,
                    s.SaleDate,
                    s.BillAmount,
                    s.PaymentStatus
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<object>>
            {
                Success = true,
                Message = "Customer statement retrieved successfully",
                Data = sales.Cast<object>().ToList()
            });
        }
    }
}
