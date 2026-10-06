using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VLCTraders.Api.Models.Domain
{
    [Table("QuotationItems")]
    public class QuotationItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QuotationItemId { get; set; }

        [Required]
        [ForeignKey("Quotation")]
        public int QuotationId { get; set; }

        [Required]
        [ForeignKey("Material")]
        public int MaterialId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal LineTotal { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Quotation Quotation { get; set; }
        public virtual Material Material { get; set; }
    }
}
