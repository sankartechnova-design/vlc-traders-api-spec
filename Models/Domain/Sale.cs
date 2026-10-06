using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VLCTraders.Api.Models.Domain
{
    [Table("Sales")]
    public class Sale
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SaleId { get; set; }

        [Required]
        [StringLength(50)]
        public string InvoiceNo { get; set; }

        [Required]
        public DateTime SaleDate { get; set; }

        [Required]
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        [Required]
        [ForeignKey("Material")]
        public int MaterialId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PricePerUnit { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BillAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SaleExpense { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PurchaseCost { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GrossProfit { get; set; }

        [StringLength(50)]
        public string PaymentStatus { get; set; } = "Pending"; // Pending, Partial, Completed

        [StringLength(50)]
        public string PaymentMethod { get; set; } // Cash, Credit, Cheque, Transfer

        [Range(0, 365)]
        public int CreditDays { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Active"; // Active, Cancelled

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }

        [StringLength(100)]
        public string CreatedBy { get; set; }

        [StringLength(100)]
        public string UpdatedBy { get; set; }

        // Navigation properties
        public virtual Customer Customer { get; set; }
        public virtual Material Material { get; set; }
    }
}
