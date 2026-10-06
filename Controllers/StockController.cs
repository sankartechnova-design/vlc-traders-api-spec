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
    public class PurchasesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PurchasesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Purchase>>>> GetPurchases()
        {
            var purchases = await _context.Purchases
                .Include(p => p.Vendor)
                .Include(p => p.Material)
                .OrderByDescending(p => p.BookingDate)
                .ToListAsync();

            return Ok(new ApiResponse<List<Purchase>>
            {
                Success = true,
                Message = "Purchases retrieved successfully",
                Data = purchases
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Purchase>>> CreatePurchase([FromBody] Purchase request)
        {
            var vendorExists = await _context.Vendors.AnyAsync(v => v.VendorId == request.VendorId);
            if (!vendorExists)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Vendor not found"
                });
            }

            var materialExists = await _context.Materials.AnyAsync(m => m.MaterialId == request.MaterialId);
            if (!materialExists)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Material not found"
                });
            }

            var duplicateInvoice = await _context.Purchases.AnyAsync(p => p.InvoiceNo == request.InvoiceNo && p.VendorId == request.VendorId);
            if (duplicateInvoice)
            {
                return Conflict(new ApiResponse
                {
                    Success = false,
                    Message = "Duplicate purchase invoice"
                });
            }

            request.Status = "Received";
            request.CreatedBy = User.Identity?.Name ?? "admin";
            request.TotalCost = request.Quantity * request.UnitCost;

            _context.Purchases.Add(request);
            await _context.SaveChangesAsync();

            return StatusCode(201, new ApiResponse<Purchase>
            {
                Success = true,
                Message = "Purchase created successfully",
                Data = request
            });
        }
    }
}
