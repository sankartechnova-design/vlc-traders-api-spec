using System.ComponentModel.DataAnnotations;

namespace VLCTraders.Api.Models.DTOs
{
    public class MaterialDto
    {
        public int MaterialId { get; set; }
        public string MaterialName { get; set; } = string.Empty;
        public int MinimumStockLevel { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class CreateMaterialRequest
    {
        [Required]
        [StringLength(150)]
        public string MaterialName { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int MinimumStockLevel { get; set; }
    }

    public class MaterialCreatedResponse
    {
        public int MaterialId { get; set; }
    }
}
