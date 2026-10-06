using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VLCTraders.Api.Models.Domain
{
    [Table("Stock")]
    public class Stock
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StockId { get; set; }

        [Required]
        [ForeignKey("Material")]
        public int MaterialId { get; set; }

        [Range(0, int.MaxValue)]
        public int PurchasedQty { get; set; }

        [Range(0, int.MaxValue)]
        public int SoldQty { get; set; }

        [Range(0, int.MaxValue)]
        public int AvailableQty { get; set; }

        [Range(0, int.MaxValue)]
        public int MinimumLevel { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "OK"; // OK, LowStock, OutOfStock

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }

        [StringLength(100)]
        public string CreatedBy { get; set; }

        [StringLength(100)]
        public string UpdatedBy { get; set; }

        // Navigation properties
        public virtual Material Material { get; set; }
    }
}
