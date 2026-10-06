using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VLCTraders.Api.Models.Domain
{
    [Table("Purchases")]
    public class Purchase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PurchaseId { get; set; }

        [Required]
        [StringLength(50)]
        public string InvoiceNo { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        public DateTime? ReceivedDate { get; set; }

        [Required]
        [ForeignKey("Vendor")]
        public int VendorId { get; set; }

        [Required]
        [ForeignKey("Material")]
        public int MaterialId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BillAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TransportAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitCost { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCost { get; set; }

        [StringLength(30)]
        public string GstNumber { get; set; }

        [StringLength(50)]
        public string PaymentMethod { get; set; } // TT, DD, Cash, Cheque

        [Range(0, 365)]
        public int CreditDays { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Received, Completed

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }

        [StringLength(100)]
        public string CreatedBy { get; set; }

        [StringLength(100)]
        public string UpdatedBy { get; set; }

        // Navigation properties
        public virtual Vendor Vendor { get; set; }
        public virtual Material Material { get; set; }
    }
}
