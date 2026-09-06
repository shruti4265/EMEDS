using EMEDS_Project.Data;
using System.ComponentModel.DataAnnotations;

namespace EMEDS_Project.Models
{
    public class Prescription
    {
        [Key]
        public int PrescriptionId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string FilePath { get; set; } = string.Empty;

        public DateTime UploadDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        [StringLength(500)]
        public string? AdminRemarks { get; set; }

        public ApplicationUser? User { get; set; }
    }
}