using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VLCTraders.Api.Data;
using VLCTraders.Api.Models.Common;
using VLCTraders.Api.Models.Domain;
using VLCTraders.Api.Models.DTOs;

namespace VLCTraders.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class MaterialsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MaterialsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<MaterialDto>>>> GetMaterials()
        {
            var items = await _context.Materials
                .OrderBy(m => m.MaterialId)
                .Select(m => new MaterialDto
                {
                    MaterialId = m.MaterialId,
                    MaterialName = m.MaterialName,
                    MinimumStockLevel = m.MinimumStockLevel,
                    Status = m.Status
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<MaterialDto>>
            {
                Success = true,
                Message = "Materials retrieved successfully",
                Data = items
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<MaterialCreatedResponse>>> CreateMaterial([FromBody] CreateMaterialRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<MaterialCreatedResponse>
                {
                    Success = false,
                    Message = "Validation failed"
                });
            }

            var exists = await _context.Materials.AnyAsync(m => m.MaterialName == request.MaterialName);
            if (exists)
            {
                return Conflict(new ApiResponse
                {
                    Success = false,
                    Message = "Material already exists"
                });
            }

            var material = new Material
            {
                MaterialName = request.MaterialName,
                MinimumStockLevel = request.MinimumStockLevel,
                Status = "Active",
                CreatedBy = "admin"
            };

            _context.Materials.Add(material);
            await _context.SaveChangesAsync();

            return StatusCode(201, new ApiResponse<MaterialCreatedResponse>
            {
                Success = true,
                Message = "Material created successfully",
                Data = new MaterialCreatedResponse { MaterialId = material.MaterialId }
            });
        }
    }
}
