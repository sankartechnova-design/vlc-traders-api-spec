using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VLCTraders.Api.Models.Domain
{
    [Table("Tasks")]
    public class UserTask
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaskId { get; set; }

        [Required]
        public DateTime TaskDate { get; set; }

        [Required]
        [StringLength(100)]
        public string AssignedTo { get; set; }

        [Required]
        [StringLength(500)]
        public string TaskName { get; set; }

        public DateTime? ReminderDate { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed, Cancelled

        [StringLength(1000)]
        public string Remarks { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }
        public DateTime? CompletedOn { get; set; }

        [StringLength(100)]
        public string CreatedBy { get; set; }

        [StringLength(100)]
        public string UpdatedBy { get; set; }
    }
}
