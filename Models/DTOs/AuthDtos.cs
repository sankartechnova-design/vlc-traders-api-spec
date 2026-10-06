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
    public class SalesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<SaleDto>>>> GetSales()
        {
            var sales = await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.Material)
                .OrderByDescending(s => s.SaleDate)
                .Select(s => new SaleDto
                {
                    SaleId = s.SaleId,
                    InvoiceNo = s.InvoiceNo,
                    SaleDate = s.SaleDate,
                    CustomerName = s.Customer.CustomerName,
                    MaterialName = s.Material.MaterialName,
                    Quantity = s.Quantity,
                    BillAmount = s.BillAmount,
                    PaymentStatus = s.PaymentStatus
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<SaleDto>>
            {
                Success = true,
                Message = "Sales retrieved successfully",
                Data = sales
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<SaleCreatedResponse>>> CreateSale([FromBody] CreateSaleRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<SaleCreatedResponse>
                {
                    Success = false,
                    Message = "Validation failed"
                });
            }

            var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == request.CustomerId);
            if (!customerExists)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Customer not found"
                });
            }

            var material = await _context.Materials.FirstOrDefaultAsync(m => m.MaterialId == request.MaterialId);
            if (material == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Material not found"
                });
            }

            var available = await _context.Stock
                .Where(s => s.MaterialId == request.MaterialId)
                .Select(s => s.AvailableQty)
                .FirstOrDefaultAsync();

            if (available < request.Quantity)
            {
                return UnprocessableEntity(new ApiResponse
                {
                    Success = false,
                    Message = "Insufficient stock",
                    Errors =
                    [
                        new ApiError { Code = "INSUFFICIENT_STOCK", Field = "quantity", Message = $"Available stock: {available}, Requested: {request.Quantity}" }
                    ]
                });
            }

            var purchaseCost = material.MinimumStockLevel > 0 ? material.MinimumStockLevel * request.PricePerUnit : request.PricePerUnit;
            var billAmount = request.Quantity * request.PricePerUnit + request.SaleExpense;
            var grossProfit = billAmount - purchaseCost;

            var sale = new Sale
            {
                InvoiceNo = request.InvoiceNo,
                SaleDate = request.SaleDate,
                CustomerId = request.CustomerId,
                MaterialId = request.MaterialId,
                Quantity = request.Quantity,
                PricePerUnit = request.PricePerUnit,
                SaleExpense = request.SaleExpense,
                PaymentStatus = request.PaymentStatus,
                PaymentMethod = request.PaymentMethod,
                CreditDays = request.CreditDays,
                BillAmount = billAmount,
                PurchaseCost = purchaseCost,
                GrossProfit = grossProfit,
                Status = "Active",
                CreatedBy = "admin"
            };

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            return StatusCode(201, new ApiResponse<SaleCreatedResponse>
            {
                Success = true,
                Message = "Sale created successfully",
                Data = new SaleCreatedResponse
                {
                    SaleId = sale.SaleId,
                    BillAmount = sale.BillAmount,
                    PurchaseCost = sale.PurchaseCost,
                    GrossProfit = sale.GrossProfit
                }
            });
        }
    }
}
