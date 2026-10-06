using System.ComponentModel.DataAnnotations;

namespace VLCTraders.Api.Models.DTOs
{
    public class CustomerDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? Address { get; set; }
        public string? ContactPerson { get; set; }
        public string Mobile { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class CreateCustomerRequest
    {
        [Required]
        [StringLength(150)]
        public string CustomerName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Location { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? ContactPerson { get; set; }

        [Required]
        [Phone]
        public string Mobile { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; }
    }

    public class UpdateCustomerRequest
    {
        public string? CustomerName { get; set; }
        public string? Location { get; set; }
        public string? Address { get; set; }
        public string? ContactPerson { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }
    }

    public class CustomerCreatedResponse
    {
        public int CustomerId { get; set; }
        public string Status { get; set; } = "Created";
    }
}
