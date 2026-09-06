using System.ComponentModel.DataAnnotations;

namespace EMEDS_Project.Models
{
    public class Inventory
    {
        [Key]
        public int InventoryId { get; set; }

        [Required]
        public int MedicineId { get; set; }

        public int? SupplierId { get; set; }

        public Supplier? Supplier { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int StockQuantity { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Reorder level cannot be negative")]
        public int ReorderLevel { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}