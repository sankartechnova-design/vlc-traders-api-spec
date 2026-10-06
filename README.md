using System.ComponentModel.DataAnnotations;

namespace VLCTraders.Api.Models.DTOs
{
    public class SaleDto
    {
        public int SaleId { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public DateTime SaleDate { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string MaterialName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal BillAmount { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
    }

    public class CreateSaleRequest
    {
        [Required]
        public string InvoiceNo { get; set; } = string.Empty;

        [Required]
        public DateTime SaleDate { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int MaterialId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal PricePerUnit { get; set; }

        [Range(0, double.MaxValue)]
        public decimal SaleExpense { get; set; }

        public string PaymentStatus { get; set; } = "Pending";
        public string PaymentMethod { get; set; } = "Credit";
        public int CreditDays { get; set; }
    }

    public class SaleCreatedResponse
    {
        public int SaleId { get; set; }
        public decimal BillAmount { get; set; }
        public decimal PurchaseCost { get; set; }
        public decimal GrossProfit { get; set; }
    }
}
