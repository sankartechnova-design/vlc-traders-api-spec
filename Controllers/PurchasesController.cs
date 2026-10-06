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
    public class VendorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VendorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Vendor>>>> GetVendors()
        {
            var vendors = await _context.Vendors.OrderBy(v => v.VendorId).ToListAsync();
            return Ok(new ApiResponse<List<Vendor>>
            {
                Success = true,
                Message = "Vendors retrieved successfully",
                Data = vendors
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Vendor>>> CreateVendor([FromBody] Vendor request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<Vendor>
                {
                    Success = false,
                    Message = "Validation failed"
                });
            }

            var exists = await _context.Vendors.AnyAsync(v => v.VendorName == request.VendorName || v.GstNumber == request.GstNumber);
            if (exists)
            {
                return Conflict(new ApiResponse
                {
                    Success = false,
                    Message = "Vendor already exists"
                });
            }

            request.Status = "Active";
            request.CreatedBy = User.Identity?.Name ?? "admin";
            _context.Vendors.Add(request);
            await _context.SaveChangesAsync();

            return StatusCode(201, new ApiResponse<Vendor>
            {
                Success = true,
                Message = "Vendor created successfully",
                Data = request
            });
        }
    }
}
