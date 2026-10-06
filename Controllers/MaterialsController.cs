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
    public class CustomersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<CustomerDto>>>> GetCustomers()
        {
            var customers = await _context.Customers
                .OrderBy(c => c.CustomerId)
                .Select(c => new CustomerDto
                {
                    CustomerId = c.CustomerId,
                    CustomerName = c.CustomerName,
                    Location = c.Location,
                    Address = c.Address,
                    ContactPerson = c.ContactPerson,
                    Mobile = c.Mobile,
                    Email = c.Email,
                    Status = c.Status
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<CustomerDto>>
            {
                Success = true,
                Message = "Customers retrieved successfully",
                Data = customers
            });
        }

        [HttpGet("{customerId:int}")]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> GetCustomer(int customerId)
        {
            var customer = await _context.Customers
                .Where(c => c.CustomerId == customerId)
                .Select(c => new CustomerDto
                {
                    CustomerId = c.CustomerId,
                    CustomerName = c.CustomerName,
                    Location = c.Location,
                    Address = c.Address,
                    ContactPerson = c.ContactPerson,
                    Mobile = c.Mobile,
                    Email = c.Email,
                    Status = c.Status
                })
                .FirstOrDefaultAsync();

            if (customer == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Customer not found"
                });
            }

            return Ok(new ApiResponse<CustomerDto>
            {
                Success = true,
                Message = "Customer retrieved successfully",
                Data = customer
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CustomerCreatedResponse>>> CreateCustomer([FromBody] CreateCustomerRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<CustomerCreatedResponse>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = ModelState
                        .SelectMany(x => x.Value!.Errors)
                        .Select(e => new ApiError { Field = x.Key, Message = e.ErrorMessage })
                        .ToList()
                });
            }

            var duplicate = await _context.Customers
                .AnyAsync(c => c.CustomerName == request.CustomerName && c.Mobile == request.Mobile);

            if (duplicate)
            {
                return Conflict(new ApiResponse
                {
                    Success = false,
                    Message = "Customer already exists"
                });
            }

            var customer = new Customer
            {
                CustomerName = request.CustomerName,
                Location = request.Location,
                Address = request.Address,
                ContactPerson = request.ContactPerson,
                Mobile = request.Mobile,
                Email = request.Email,
                Status = "Active",
                CreatedBy = "admin"
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return StatusCode(201, new ApiResponse<CustomerCreatedResponse>
            {
                Success = true,
                Message = "Customer created successfully",
                Data = new CustomerCreatedResponse
                {
                    CustomerId = customer.CustomerId,
                    Status = "Created"
                }
            });
        }

        [HttpPut("{customerId:int}")]
        public async Task<IActionResult> UpdateCustomer(int customerId, [FromBody] UpdateCustomerRequest request)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Customer not found"
                });
            }

            customer.CustomerName = request.CustomerName ?? customer.CustomerName;
            customer.Location = request.Location ?? customer.Location;
            customer.Address = request.Address ?? customer.Address;
            customer.ContactPerson = request.ContactPerson ?? customer.ContactPerson;
            customer.Mobile = request.Mobile ?? customer.Mobile;
            customer.Email = request.Email ?? customer.Email;
            customer.UpdatedOn = DateTime.UtcNow;
            customer.UpdatedBy = "admin";

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Customer updated successfully"
            });
        }

        [HttpDelete("{customerId:int}")]
        public async Task<IActionResult> DeleteCustomer(int customerId)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Customer not found"
                });
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
