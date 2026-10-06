using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VLCTraders.Api.Models.Domain
{
    [Table("AuditLogs")]
    public class AuditLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AuditId { get; set; }

        [StringLength(100)]
        public string UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string EntityName { get; set; }

        public int EntityId { get; set; }

        [Required]
        [StringLength(50)]
        public string Action { get; set; } // Create, Update, Delete

        [Column(TypeName = "longtext")]
        public string OldValue { get; set; }

        [Column(TypeName = "longtext")]
        public string NewValue { get; set; }

        [StringLength(45)]
        public string IpAddress { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
