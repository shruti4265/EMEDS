//using EMEDS_Project.Data;
//using EMEDS_Project.Models;
//using System.ComponentModel.DataAnnotations;

//namespace EMEDS_Project.Models
//{
//    public class Prescription
//    {
//        [Key]
//        public int PrescriptionId { get; set; }

//        [Required]
//        public string UserId { get; set; } = string.Empty;

//        [Required]
//        public int MedicineId { get; set; }

//        public Medicine? Medicine { get; set; }

//        [Required]
//        public string FilePath { get; set; } = string.Empty;

//        public DateTime UploadDate { get; set; } = DateTime.Now;

//        [Required]
//        [StringLength(50)]
//        public string Status { get; set; } = "Pending";

//        [StringLength(500)]
//        public string? AdminRemarks { get; set; }

//        public ApplicationUser? User { get; set; }
//    }
//}




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
        public int MedicineId { get; set; }

        public Medicine? Medicine { get; set; }

        // Prescription can be connected to a placed order.
        // Nullable so older prescriptions or medicine-page uploads
        // do not immediately break.
        public int? OrderId { get; set; }

        public Order? Order { get; set; }

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